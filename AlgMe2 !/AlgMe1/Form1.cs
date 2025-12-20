using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlgMe1
{
    public partial class Form1 : Form
    {
        private GraphBuilder graphBuilder;
        private StateSpaceBuilder stateSpaceBuilder;
        private List<SimulationPoint> simulationResults;
        private List<Component> components;
        private Dictionary<string, TextBox> initialConditionTextBoxes;
        private double[] initialConditions;

        public Form1()
        {
            InitializeComponent();
            components = new List<Component>();
            simulationResults = new List<SimulationPoint>();
            initialConditionTextBoxes = new Dictionary<string, TextBox>();
            initialConditions = null;
        }

        private void btnParseCircuit_Click(object sender, EventArgs e)
        {
            try
            {
                components.Clear();
                string[] lines = txtCircuitDescription.Lines;

                txtLog.AppendText("\r\n=== ПАРСИНГ СХЕМЫ ===\r\n");

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("//"))
                        continue;

                    var comp = ParseComponent(line.Trim());
                    if (comp != null)
                    {
                        components.Add(comp);
                        txtLog.AppendText($"  ✓ {comp}\r\n");
                    }
                }

                txtLog.AppendText($"\r\nВсего загружено компонентов: {components.Count}\r\n");

                int rCount = components.Count(c => c.Type == ComponentType.Resistor);
                int lCount = components.Count(c => c.Type == ComponentType.Inductor);
                int cCount = components.Count(c => c.Type == ComponentType.Capacitor);
                int vCount = components.Count(c => c.Type == ComponentType.VoltageSource);
                int iCount = components.Count(c => c.Type == ComponentType.CurrentSource);

                txtLog.AppendText($"  Резисторов: {rCount}\r\n");
                txtLog.AppendText($"  Индуктивностей: {lCount}\r\n");
                txtLog.AppendText($"  Конденсаторов: {cCount}\r\n");
                txtLog.AppendText($"  Источников напряжения: {vCount}\r\n");
                txtLog.AppendText($"  Источников тока: {iCount}\r\n");

                MessageBox.Show(
                    $"Схема загружена успешно!\n\n" +
                    $"Компонентов: {components.Count}\n" +
                    $"R: {rCount}, L: {lCount}, C: {cCount}\n" +
                    $"V: {vCount}, I: {iCount}",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при разборе схемы:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                txtLog.AppendText($"\r\n❌ ОШИБКА: {ex.Message}\r\n");
            }
        }

        private void btnBuildGraph_Click(object sender, EventArgs e)
        {
            try
            {
                if (components.Count == 0)
                {
                    MessageBox.Show("Сначала загрузите схему!", "Ошибка",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                txtLog.AppendText("\r\n=== ПОСТРОЕНИЕ ГРАФА ===\r\n");

                graphBuilder = new GraphBuilder();

                txtLog.AppendText("Добавление компонентов в граф:\r\n");
                foreach (var comp in components)
                {
                    try
                    {
                        graphBuilder.AddComponent(comp);
                        txtLog.AppendText($"  ✓ {comp}\r\n");
                    }
                    catch (Exception ex)
                    {
                        txtLog.AppendText($"  ✗ Ошибка при добавлении {comp.Name}: {ex.Message}\r\n");
                        throw;
                    }
                }

                txtLog.AppendText($"\nУзлов в графе: {graphBuilder.Nodes.Count}\r\n");
                txtLog.AppendText($"Рёбер в графе: {graphBuilder.Edges.Count}\r\n");

                graphBuilder.BuildSpanningTree();

                txtLog.AppendText("\r\n" + graphBuilder.GetTreeInfo());

                MessageBox.Show("Граф построен успешно!", "Успех",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при построении графа:\n{ex.Message}\n\n{ex.StackTrace}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                txtLog.AppendText($"\r\n❌ ОШИБКА: {ex.Message}\r\n");
                txtLog.AppendText($"Stack trace: {ex.StackTrace}\r\n");
            }
        }

        private void btnBuildEquations_Click(object sender, EventArgs e)
        {
            try
            {
                if (graphBuilder == null)
                {
                    MessageBox.Show(
                        "Сначала постройте граф!",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                txtLog.AppendText("\r\n=== ФОРМИРОВАНИЕ УРАВНЕНИЙ ===\r\n");

                int capacitorCount = components.Count(c => c.Type == ComponentType.Capacitor);
                int inductorCount = components.Count(c => c.Type == ComponentType.Inductor);

                if (capacitorCount == 0 && inductorCount == 0)
                {
                    txtLog.AppendText("⚠️ Схема не содержит реактивных элементов.\r\n");
                    txtLog.AppendText("Применяется метод узловых потенциалов.\r\n");

                    MessageBox.Show(
                        "⚠️ ВНИМАНИЕ!\n\n" +
                        "Схема не содержит реактивных элементов (L или C).\n" +
                        "Метод переменных состояния не применим.\n\n" +
                        "Для резистивных цепей используется метод узловых потенциалов.\n" +
                        "Решение находится мгновенно из алгебраических уравнений.",
                        "Резистивная цепь",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    SolveResistiveCircuit();
                    return;
                }

                stateSpaceBuilder = new StateSpaceBuilder(graphBuilder);
                stateSpaceBuilder.BuildStateSpaceMatrices();

                txtLog.AppendText(stateSpaceBuilder.GetMatrixInfo());

                CreateInitialConditionFields();

                grpInitialConditions.Visible = true;

                MessageBox.Show(
                    $"Уравнения состояния сформированы!\n\n" +
                    $"Переменных состояния: {stateSpaceBuilder.StateCount}\n" +
                    $"Входных воздействий: {stateSpaceBuilder.InputCount}",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при формировании уравнений:\n{ex.Message}\n\n{ex.StackTrace}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                txtLog.AppendText($"\r\n❌ ОШИБКА: {ex.Message}\r\n");
            }
        }

        private void btnSimulate_Click(object sender, EventArgs e)
        {
            try
            {
                if (stateSpaceBuilder == null)
                {
                    MessageBox.Show(
                        "Сначала сформируйте уравнения!",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                txtLog.AppendText("\r\n=== ЗАПУСК СИМУЛЯЦИИ ===\r\n");

                double totalTime = double.Parse(txtTotalTime.Text.Replace('.', ','));
                double timeStep = double.Parse(txtTimeStep.Text.Replace('.', ','));

                txtLog.AppendText($"Время моделирования: {totalTime} с\r\n");
                txtLog.AppendText($"Шаг интегрирования: {timeStep} с\r\n");

                initialConditions = ParseInitialConditions();

                txtLog.AppendText("\r\nНачальные условия:\r\n");
                for (int i = 0; i < initialConditions.Length; i++)
                {
                    txtLog.AppendText($"  {stateSpaceBuilder.StateVariables[i]}(0) = {initialConditions[i]:G6}\r\n");
                }

                var inputValues = new List<double>();

                foreach (var vs in components.Where(c => c.Type == ComponentType.VoltageSource))
                {
                    inputValues.Add(vs.Value);
                    txtLog.AppendText($"  Источник {vs.Name} = {vs.Value} В\r\n");
                }

                foreach (var cs in components.Where(c => c.Type == ComponentType.CurrentSource))
                {
                    inputValues.Add(cs.Value);
                    txtLog.AppendText($"  Источник {cs.Name} = {cs.Value} А\r\n");
                }

                var simulator = new Simulator(stateSpaceBuilder, inputValues);

                txtLog.AppendText("\r\nВыполняется численное интегрирование...\r\n");
                Application.DoEvents();

                simulationResults = simulator.RunSimulation(totalTime, timeStep, initialConditions);

                txtLog.AppendText($"✓ Симуляция завершена!\r\n");
                txtLog.AppendText($"Точек данных: {simulationResults.Count}\r\n");

                if (simulationResults.Count > 0)
                {
                    var lastPoint = simulationResults[simulationResults.Count - 1];
                    txtLog.AppendText("\r\nКонечные значения (установившийся режим):\r\n");
                    for (int i = 0; i < lastPoint.State.Length; i++)
                    {
                        txtLog.AppendText($"  {stateSpaceBuilder.StateVariables[i]}(T) = {lastPoint.State[i]:G6}\r\n");
                    }
                }

                UpdateResultsGrid();
                panelGraph.Invalidate();

                MessageBox.Show(
                    $"Симуляция завершена успешно!\n\n" +
                    $"Точек данных: {simulationResults.Count}\n" +
                    $"Время симуляции: {totalTime} с",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (FormatException ex)
            {
                MessageBox.Show(
                    $"Ошибка в начальных условиях:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                txtLog.AppendText($"\r\n❌ ОШИБКА: {ex.Message}\r\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при симуляции:\n{ex.Message}\n\n{ex.StackTrace}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                txtLog.AppendText($"\r\n❌ ОШИБКА: {ex.Message}\r\n");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
            txtCircuitDescription.Clear();
            components.Clear();
            simulationResults.Clear();
            graphBuilder = null;
            stateSpaceBuilder = null;
            dgvResults.Rows.Clear();
            dgvResults.Columns.Clear();
            panelGraph.Invalidate();
            grpInitialConditions.Visible = false;
            initialConditionTextBoxes.Clear();
            panelInitialConditions.Controls.Clear();
            initialConditions = null;

            txtLog.AppendText("=== ВСЕ ДАННЫЕ ОЧИЩЕНЫ ===\r\n");
        }

        private void btnLoadExample_Click(object sender, EventArgs e)
        {
            var exampleDialog = new Form();
            exampleDialog.Text = "Выберите пример схемы";
            exampleDialog.Size = new Size(600, 500);
            exampleDialog.StartPosition = FormStartPosition.CenterParent;
            exampleDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            exampleDialog.MaximizeBox = false;
            exampleDialog.MinimizeBox = false;

            var listBox = new ListBox();
            listBox.Dock = DockStyle.Fill;
            listBox.Font = new Font("Consolas", 9);
            listBox.ItemHeight = 20;

            var examples = new Dictionary<string, (string circuit, string ic, string time, string step, string desc)>
            {
                ["1. RC-цепь (заряд конденсатора)"] = (
                    "R R1 1 2 1000\r\nC C1 2 0 0.000001\r\nV V1 1 0 5",
                    "IC Uc_C1 0",
                    "0.005",
                    "0.00001",
                    "Классическая RC-цепь. Конденсатор заряжается через резистор."
                ),
                ["2. RC-цепь (разряд конденсатора)"] = (
                    "R R1 1 0 1000\r\nC C1 1 0 0.000001\r\nV V1 1 0 0",
                    "IC Uc_C1 5.0",
                    "0.005",
                    "0.00001",
                    "Конденсатор с начальным зарядом 5В разряжается через резистор."
                ),
                ["3. RLC последовательный (затухающие колебания)"] = (
                    "R R1 1 2 10\r\nL L1 2 3 0.001\r\nC C1 3 0 0.00001\r\nV V1 1 0 10",
                    "IC Uc_C1 0\r\nIC IL_L1 0",
                    "0.002",
                    "0.000001",
                    "RLC-контур с затуханием. Колебания с частотой около 10 кГц."
                ),
                ["4. RLC (свободные колебания от начальных условий)"] = (
                    "R R1 1 2 10\r\nL L1 2 3 0.001\r\nC C1 3 0 0.00001\r\nV V1 1 0 10",
                    "IC Uc_C1 10.0\r\nIC IL_L1 0",
                    "0.002",
                    "0.000001",
                    "Затухание колебаний в RLC-контуре с начальным зарядом конденсатора."
                ),
                ["5. LC-контур (незатухающие колебания)"] = (
                    "L L1 1 2 0.001\r\nC C1 2 0 0.00001\r\nI I1 1 0 0.01",
                    "IC Uc_C1 10.0\r\nIC IL_L1 0",
                    "0.002",
                    "0.0000001",
                    "Идеальный резонансный контур без потерь. Энергия колеблется между L и C."
                ),
                ["6. RL-цепь (нарастание тока)"] = (
                    "R R1 1 2 100\r\nL L1 2 0 0.01\r\nV V1 1 0 10",
                    "IC IL_L1 0",
                    "0.0005",
                    "0.000001",
                    "Индуктивная цепь. Ток нарастает экспоненциально до I = V/R."
                ),
                ["7. Двухкаскадный RC-фильтр"] = (
                    "R R1 1 2 1000\r\nC C1 2 0 0.000001\r\nR R2 2 3 2000\r\nC C2 3 0 0.000002\r\nV V1 1 0 5",
                    "IC Uc_C1 0\r\nIC Uc_C2 0",
                    "0.01",
                    "0.00001",
                    "Два каскада RC-фильтра. Uc1 заряжается быстрее, Uc2 медленнее."
                ),
                ["8. Схема из лекции (сложная топология)"] = (
                    "R R1 1 2 100\r\nR R2 0 1 200\r\nL L1 2 0 0.001\r\nC C1 1 0 0.00001\r\nI J1 0 1 0.01",
                    "IC Uc_C1 0\r\nIC IL_L1 0",
                    "0.011",
                    "0.000001",
                    "Пример из методического пособия. Смешанная топология."
                ),
                ["9. Три каскада RC (многокаскадный фильтр)"] = (
                    "R R1 1 2 1000\r\nC C1 2 0 0.000001\r\nR R2 2 3 1000\r\nC C2 3 0 0.000001\r\nR R3 3 4 1000\r\nC C3 4 0 0.000001\r\nV V1 1 0 10",
                    "IC Uc_C1 0\r\nIC Uc_C2 0\r\nIC Uc_C3 0",
                    "0.01",
                    "0.00001",
                    "Три последовательных RC-каскада. Каскадное нарастание напряжений."
                ),
                ["10. RLC с источником тока"] = (
                    "R R1 1 0 100\r\nL L1 1 2 0.01\r\nC C1 2 0 0.00001\r\nI I1 0 1 0.05",
                    "IC Uc_C1 0\r\nIC IL_L1 0",
                    "0.002",
                    "0.000001",
                    "RLC-контур с источником тока. Колебательный процесс."
                ),
                ["11. МДП инвертор (0→1)"] = (
                    "V Vdd 1 0 5\r\n" +
                    "R R1 1 2 10000\r\n" +
                    "C Csi 2 0 0.000000001\r\n" +
                    "C Czs 1 2 0.0000000005\r\n" +
                    "C Cn 2 0 0.000000002\r\n" +
                    "VCCS S1 2 0 3 0 0.00002\r\n" + 
                    "V Vin 3 0 0",
                    "IC Uout_node2 0.2", 
                    "0.00001",
                    "0.0000001",
                    "Переключение 0→1: транзистор закрывается, выход растет до 5В"
                ),
                                ["12. МДП инвертор (1→0)"] = (
                    "V Vdd 1 0 5\r\n" +
                    "R R1 1 2 10000\r\n" +
                    "C Csi 2 0 0.000000001\r\n" +
                    "C Czs 1 2 0.0000000005\r\n" +
                    "C Cn 2 0 0.000000002\r\n" +
                    "VCCS S1 2 0 3 0 0.00002\r\n" +  
                    "V Vin 3 0 5",  
                    "IC Uout_node2 5.0", 
                    "0.00001",
                    "0.0000001",
                    "Переключение 1→0: транзистор открывается, выход падает"
                )
            };

            foreach (var key in examples.Keys)
            {
                listBox.Items.Add(key);
            }

            var panelDesc = new Panel();
            panelDesc.Dock = DockStyle.Bottom;
            panelDesc.Height = 80;
            panelDesc.BorderStyle = BorderStyle.FixedSingle;

            var lblDesc = new Label();
            lblDesc.Dock = DockStyle.Fill;
            lblDesc.Font = new Font("Arial", 9);
            lblDesc.Padding = new Padding(5);
            lblDesc.Text = "Выберите пример из списка";
            panelDesc.Controls.Add(lblDesc);

            listBox.SelectedIndexChanged += (s, ev) =>
            {
                if (listBox.SelectedItem != null)
                {
                    var selected = examples[listBox.SelectedItem.ToString()];
                    lblDesc.Text = selected.desc;
                }
            };

            var btnPanel = new Panel();
            btnPanel.Dock = DockStyle.Bottom;
            btnPanel.Height = 50;

            var btnOK = new Button();
            btnOK.Text = "Загрузить";
            btnOK.Size = new Size(120, 35);
            btnOK.Location = new Point(350, 7);
            btnOK.BackColor = Color.LightGreen;
            btnOK.Font = new Font("Arial", 9, FontStyle.Bold);
            btnOK.Click += (s, ev) =>
            {
                if (listBox.SelectedItem != null)
                {
                    var selected = examples[listBox.SelectedItem.ToString()];
                    txtCircuitDescription.Text = selected.circuit;
                    txtInitialConditionsText.Text = selected.ic;
                    txtTotalTime.Text = selected.time;
                    txtTimeStep.Text = selected.step;

                    txtLog.AppendText($"\r\n=== ЗАГРУЖЕН ПРИМЕР ===\r\n");
                    txtLog.AppendText($"{listBox.SelectedItem}\r\n");
                    txtLog.AppendText($"{selected.desc}\r\n");

                    exampleDialog.Close();
                }
            };

            var btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Size = new Size(120, 35);
            btnCancel.Location = new Point(480, 7);
            btnCancel.BackColor = Color.LightCoral;
            btnCancel.Font = new Font("Arial", 9, FontStyle.Bold);
            btnCancel.Click += (s, ev) => exampleDialog.Close();

            btnPanel.Controls.Add(btnOK);
            btnPanel.Controls.Add(btnCancel);

            exampleDialog.Controls.Add(listBox);
            exampleDialog.Controls.Add(panelDesc);
            exampleDialog.Controls.Add(btnPanel);

            exampleDialog.ShowDialog();
        }

        private void CreateInitialConditionFields()
        {
            panelInitialConditions.Controls.Clear();
            initialConditionTextBoxes.Clear();

            int yPosition = 10;

            for (int i = 0; i < stateSpaceBuilder.StateVariables.Count; i++)
            {
                string varName = stateSpaceBuilder.StateVariables[i];

                Label lbl = new Label();
                lbl.Text = $"{varName}(0) =";
                lbl.Location = new Point(10, yPosition + 3);
                lbl.Width = 100;
                lbl.Font = new Font("Consolas", 9);
                panelInitialConditions.Controls.Add(lbl);

                TextBox txt = new TextBox();
                txt.Name = $"txt_{varName}";
                txt.Location = new Point(120, yPosition);
                txt.Width = 100;
                txt.Text = "0";
                txt.Font = new Font("Consolas", 9);
                panelInitialConditions.Controls.Add(txt);

                Label lblUnit = new Label();
                if (varName.StartsWith("Uc_"))
                    lblUnit.Text = "В";
                else if (varName.StartsWith("IL_"))
                    lblUnit.Text = "А";
                lblUnit.Location = new Point(230, yPosition + 3);
                lblUnit.Width = 30;
                panelInitialConditions.Controls.Add(lblUnit);

                initialConditionTextBoxes[varName] = txt;

                yPosition += 30;
            }

            txtLog.AppendText($"\r\nСоздано {stateSpaceBuilder.StateVariables.Count} полей для начальных условий.\r\n");
        }

        private void rbICMode_CheckedChanged(object sender, EventArgs e)
        {
            if (rbICFieldsMode.Checked)
            {
                panelInitialConditions.Visible = true;
                txtInitialConditionsText.Visible = false;
            }
            else
            {
                panelInitialConditions.Visible = false;
                txtInitialConditionsText.Visible = true;
            }
        }

        private void btnResetIC_Click(object sender, EventArgs e)
        {
            if (rbICFieldsMode.Checked)
            {
                foreach (var txtBox in initialConditionTextBoxes.Values)
                {
                    txtBox.Text = "0";
                }
            }
            else
            {
                txtInitialConditionsText.Clear();
                txtInitialConditionsText.Text = "// Примеры:\r\n// IC Uc_C1 5.0\r\n// IC IL_L1 0.1\r\n";
            }

            txtLog.AppendText("Начальные условия сброшены в 0.\r\n");
        }

        private double[] ParseInitialConditions()
        {
            if (stateSpaceBuilder == null || stateSpaceBuilder.StateCount == 0)
                return new double[0];

            double[] ic = new double[stateSpaceBuilder.StateCount];

            if (rbICFieldsMode.Checked)
            {
                for (int i = 0; i < stateSpaceBuilder.StateVariables.Count; i++)
                {
                    string varName = stateSpaceBuilder.StateVariables[i];

                    if (initialConditionTextBoxes.ContainsKey(varName))
                    {
                        string text = initialConditionTextBoxes[varName].Text;
                        if (double.TryParse(text.Replace('.', ','), out double value))
                        {
                            ic[i] = value;
                        }
                        else
                        {
                            throw new FormatException($"Неверное значение для {varName}: {text}");
                        }
                    }
                }
            }
            else
            {
                string[] lines = txtInitialConditionsText.Lines;

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("//"))
                        continue;

                    string[] parts = line.Split(new[] { ' ', '\t' },
                                               StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length >= 3 && parts[0].ToUpper() == "IC")
                    {
                        string varName = parts[1];
                        double value = double.Parse(parts[2].Replace('.', ','));

                        int idx = stateSpaceBuilder.StateVariables.IndexOf(varName);
                        if (idx >= 0)
                        {
                            ic[idx] = value;
                        }
                        else
                        {
                            txtLog.AppendText($"⚠️ Предупреждение: переменная {varName} не найдена.\r\n");
                        }
                    }
                }
            }

            return ic;
        }

        private Component ParseComponent(string line)
        {
            string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 5)
                throw new FormatException($"Неверный формат строки: {line}");

            string typeStr = parts[0].ToUpper();
            string name = parts[1];

            if (typeStr == "VCCS" || typeStr == "VCIS" || typeStr == "G")
            {
                if (parts.Length < 7)
                    throw new FormatException($"VCCS требует 7 параметров: {line}\nФормат: VCCS имя узел+ узел- управ+ управ- крутизна");

                int node1 = int.Parse(parts[2]);
                int node2 = int.Parse(parts[3]);
                int ctrlNode1 = int.Parse(parts[4]);
                int ctrlNode2 = int.Parse(parts[5]);
                double gain = double.Parse(parts[6].Replace('.', ','));

                return new Component(name, ComponentType.VCCS, node1, node2, ctrlNode1, ctrlNode2, gain);
            }

            else if (typeStr == "VCVS" || typeStr == "E")
            {
                if (parts.Length < 7)
                    throw new FormatException($"VCVS требует 7 параметров: {line}\nФормат: VCVS имя узел+ узел- управ+ управ- коэфф");

                int node1 = int.Parse(parts[2]);
                int node2 = int.Parse(parts[3]);
                int ctrlNode1 = int.Parse(parts[4]);
                int ctrlNode2 = int.Parse(parts[5]);
                double gain = double.Parse(parts[6].Replace('.', ','));

                return new Component(name, ComponentType.VCVS, node1, node2, ctrlNode1, ctrlNode2, gain);
            }

            int n1 = int.Parse(parts[2]);
            int n2 = int.Parse(parts[3]);
            double value = double.Parse(parts[4].Replace('.', ','));

            ComponentType type;
            switch (typeStr)
            {
                case "R": type = ComponentType.Resistor; break;
                case "C": type = ComponentType.Capacitor; break;
                case "L": type = ComponentType.Inductor; break;
                case "V": type = ComponentType.VoltageSource; break;
                case "I": type = ComponentType.CurrentSource; break;
                default:
                    throw new FormatException($"Неизвестный тип компонента: {typeStr}");
            }

            return new Component(name, type, n1, n2, value);
        }

        private void UpdateResultsGrid()
        {
            if (simulationResults.Count == 0) return;

            dgvResults.Rows.Clear();
            dgvResults.Columns.Clear();

            dgvResults.Columns.Add("Time", "Время (с)");

            if (stateSpaceBuilder != null)
            {
                foreach (var varName in stateSpaceBuilder.StateVariables)
                {
                    dgvResults.Columns.Add(varName, varName);
                }
            }

            int step = Math.Max(1, simulationResults.Count / 100);

            for (int i = 0; i < simulationResults.Count; i += step)
            {
                var point = simulationResults[i];
                var row = new List<object> { point.Time.ToString("E4") };

                foreach (var val in point.State)
                {
                    row.Add(val.ToString("E4"));
                }

                dgvResults.Rows.Add(row.ToArray());
            }

            foreach (DataGridViewColumn col in dgvResults.Columns)
            {
                col.Width = 120;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            dgvResults.Columns[0].DefaultCellStyle.BackColor = Color.LightYellow;
        }

        private void panelGraph_Paint(object sender, PaintEventArgs e)
        {
            if (simulationResults == null || simulationResults.Count == 0)
            {
                Graphics g = e.Graphics;
                g.Clear(Color.White);

                Font font1 = new Font("Arial", 12);
                string msg = "График появится после выполнения симуляции";
                SizeF msgSize = g.MeasureString(msg, font1);

                g.DrawString(msg, font1, Brushes.Gray,
                    (panelGraph.Width - msgSize.Width) / 2,
                    (panelGraph.Height - msgSize.Height) / 2);

                return;
            }

            Graphics graphics = e.Graphics;
            graphics.Clear(Color.White);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int width = panelGraph.Width - 100;
            int height = panelGraph.Height - 100;
            int offsetX = 70;
            int offsetY = 40;

            Pen axisPen = new Pen(Color.Black, 2);
            graphics.DrawLine(axisPen, offsetX, offsetY + height, offsetX, offsetY);
            graphics.DrawLine(axisPen, offsetX, offsetY + height, offsetX + width, offsetY + height);

            graphics.DrawLine(axisPen, offsetX, offsetY, offsetX - 5, offsetY + 10);
            graphics.DrawLine(axisPen, offsetX, offsetY, offsetX + 5, offsetY + 10);
            graphics.DrawLine(axisPen, offsetX + width, offsetY + height, offsetX + width - 10, offsetY + height - 5);
            graphics.DrawLine(axisPen, offsetX + width, offsetY + height, offsetX + width - 10, offsetY + height + 5);

            Font font = new Font("Arial", 10, FontStyle.Bold);
            graphics.DrawString("t, с", font, Brushes.Black, offsetX + width - 30, offsetY + height + 5);
            graphics.DrawString("Переменные\nсостояния", font, Brushes.Black, 5, offsetY - 20);

            if (stateSpaceBuilder == null || stateSpaceBuilder.StateCount == 0)
                return;

            int stateCount = stateSpaceBuilder.StateCount;
            Color[] colors = { Color.Blue, Color.Red, Color.Green, Color.Purple,
                      Color.Orange, Color.Brown, Color.Magenta, Color.Teal };


            double minVal = double.MaxValue;
            double maxVal = double.MinValue;
            double maxTime = simulationResults[simulationResults.Count - 1].Time;

            foreach (var point in simulationResults)
            {
                foreach (var val in point.State)
                {
                    minVal = Math.Min(minVal, val);
                    maxVal = Math.Max(maxVal, val);
                }
            }

            double range = maxVal - minVal;
            if (range < 1e-10) range = 1;

            Pen gridPen = new Pen(Color.LightGray, 1);
            gridPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

            for (int i = 0; i <= 10; i++)
            {
                int y = offsetY + (height * i / 10);
                graphics.DrawLine(gridPen, offsetX, y, offsetX + width, y);
                double val = maxVal - (range * i / 10);
                graphics.DrawString(val.ToString("E2"), new Font("Arial", 8), Brushes.Black, 5, y - 7);
            }

            for (int i = 0; i <= 10; i++)
            {
                int x = offsetX + (width * i / 10);
                graphics.DrawLine(gridPen, x, offsetY, x, offsetY + height);
                double t = maxTime * i / 10;
                graphics.DrawString(t.ToString("E2"), new Font("Arial", 8), Brushes.Black, x - 20, offsetY + height + 5);
            }

            for (int stateIdx = 0; stateIdx < Math.Min(stateCount, colors.Length); stateIdx++)
            {
                Pen pen = new Pen(colors[stateIdx], 2);

                for (int i = 0; i < simulationResults.Count - 1; i++)
                {
                    double val1 = simulationResults[i].State[stateIdx];
                    double val2 = simulationResults[i + 1].State[stateIdx];
                    double t1 = simulationResults[i].Time;
                    double t2 = simulationResults[i + 1].Time;

                    int x1 = offsetX + (int)((t1 / maxTime) * width);
                    int y1 = offsetY + height - (int)(((val1 - minVal) / range) * height);
                    int x2 = offsetX + (int)((t2 / maxTime) * width);
                    int y2 = offsetY + height - (int)(((val2 - minVal) / range) * height);

                    y1 = Math.Max(offsetY, Math.Min(offsetY + height, y1));
                    y2 = Math.Max(offsetY, Math.Min(offsetY + height, y2));

                    graphics.DrawLine(pen, x1, y1, x2, y2);
                }
            }

            int legendX = offsetX + width - 180;
            int legendY = offsetY + 30;

            graphics.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 240)),
                legendX - 5, legendY - 5, 175, stateCount * 20 + 15);
            graphics.DrawRectangle(new Pen(Color.Gray),
                legendX - 5, legendY - 5, 175, stateCount * 20 + 15);

            for (int i = 0; i < Math.Min(stateCount, colors.Length); i++)
            {
                graphics.FillRectangle(new SolidBrush(colors[i]), legendX, legendY, 30, 12);
                graphics.DrawRectangle(Pens.Black, legendX, legendY, 30, 12);

                string varName = i < stateSpaceBuilder.StateVariables.Count ?
                                stateSpaceBuilder.StateVariables[i] : $"x{i + 1}";
                graphics.DrawString(varName, new Font("Consolas", 9), Brushes.Black, legendX + 35, legendY - 2);
                legendY += 20;
            }
        }

        private void SolveResistiveCircuit()
        {
            txtLog.AppendText("\r\n=== РЕШЕНИЕ РЕЗИСТИВНОЙ ЦЕПИ ===\r\n");
            txtLog.AppendText("Метод узловых потенциалов\r\n\r\n");

            var nodes = new HashSet<int>();
            foreach (var comp in components)
            {
                if (comp.Node1 != 0) nodes.Add(comp.Node1);
                if (comp.Node2 != 0) nodes.Add(comp.Node2);
            }

            int n = nodes.Count;
            if (n == 0)
            {
                txtLog.AppendText("Все компоненты подключены к земле!\r\n");
                return;
            }

            double[,] G = new double[n, n];
            double[] I = new double[n];
            var nodeList = nodes.OrderBy(x => x).ToList();

            foreach (var res in components.Where(c => c.Type == ComponentType.Resistor))
            {
                double g = 1.0 / res.Value;

                if (res.Node1 != 0 && res.Node2 != 0)
                {
                    int i = nodeList.IndexOf(res.Node1);
                    int j = nodeList.IndexOf(res.Node2);

                    G[i, i] += g;
                    G[j, j] += g;
                    G[i, j] -= g;
                    G[j, i] -= g;
                }
                else if (res.Node1 != 0)
                {
                    int i = nodeList.IndexOf(res.Node1);
                    G[i, i] += g;
                }
                else if (res.Node2 != 0)
                {
                    int i = nodeList.IndexOf(res.Node2);
                    G[i, i] += g;
                }
            }

            foreach (var curr in components.Where(c => c.Type == ComponentType.CurrentSource))
            {
                if (curr.Node1 != 0)
                {
                    int i = nodeList.IndexOf(curr.Node1);
                    I[i] -= curr.Value;
                }
                if (curr.Node2 != 0)
                {
                    int i = nodeList.IndexOf(curr.Node2);
                    I[i] += curr.Value;
                }
            }

            foreach (var volt in components.Where(c => c.Type == ComponentType.VoltageSource))
            {
                if (volt.Node1 == 0 && volt.Node2 != 0)
                {
                    int i = nodeList.IndexOf(volt.Node2);
                    for (int j = 0; j < n; j++)
                        G[i, j] = 0;
                    G[i, i] = 1;
                    I[i] = volt.Value;
                }
                else if (volt.Node2 == 0 && volt.Node1 != 0)
                {
                    int i = nodeList.IndexOf(volt.Node1);
                    for (int j = 0; j < n; j++)
                        G[i, j] = 0;
                    G[i, i] = 1;
                    I[i] = volt.Value;
                }
            }

            double[] V = SolveLinearSystem(G, I, n);

            txtLog.AppendText("Узловые потенциалы:\r\n");
            for (int i = 0; i < n; i++)
            {
                txtLog.AppendText($"  V{nodeList[i]} = {V[i]:F6} В\r\n");
            }

            txtLog.AppendText("\r\nТоки через компоненты:\r\n");
            foreach (var res in components.Where(c => c.Type == ComponentType.Resistor))
            {
                double v1 = res.Node1 == 0 ? 0 : V[nodeList.IndexOf(res.Node1)];
                double v2 = res.Node2 == 0 ? 0 : V[nodeList.IndexOf(res.Node2)];
                double current = (v1 - v2) / res.Value;
                txtLog.AppendText($"  I_{res.Name} = {current:F6} А (от узла {res.Node1} к {res.Node2})\r\n");
            }

            txtLog.AppendText("\r\nМощности:\r\n");
            double totalPower = 0;
            foreach (var res in components.Where(c => c.Type == ComponentType.Resistor))
            {
                double v1 = res.Node1 == 0 ? 0 : V[nodeList.IndexOf(res.Node1)];
                double v2 = res.Node2 == 0 ? 0 : V[nodeList.IndexOf(res.Node2)];
                double current = (v1 - v2) / res.Value;
                double power = current * current * res.Value;
                totalPower += power;
                txtLog.AppendText($"  P_{res.Name} = {power:F6} Вт\r\n");
            }
            txtLog.AppendText($"\r\n  Общая рассеиваемая мощность = {totalPower:F6} Вт\r\n");
        }

        private double[] SolveLinearSystem(double[,] A, double[] b, int n)
        {
            double[,] Ab = new double[n, n + 1];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    Ab[i, j] = A[i, j];
                Ab[i, n] = b[i];
            }

            for (int k = 0; k < n; k++)
            {
                int maxRow = k;
                for (int i = k + 1; i < n; i++)
                {
                    if (Math.Abs(Ab[i, k]) > Math.Abs(Ab[maxRow, k]))
                        maxRow = i;
                }

                if (maxRow != k)
                {
                    for (int j = 0; j <= n; j++)
                    {
                        double temp = Ab[k, j];
                        Ab[k, j] = Ab[maxRow, j];
                        Ab[maxRow, j] = temp;
                    }
                }

                for (int i = k + 1; i < n; i++)
                {
                    if (Math.Abs(Ab[k, k]) < 1e-10) continue;
                    double factor = Ab[i, k] / Ab[k, k];
                    for (int j = k; j <= n; j++)
                        Ab[i, j] -= factor * Ab[k, j];
                }
            }

            double[] x = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                x[i] = Ab[i, n];
                for (int j = i + 1; j < n; j++)
                    x[i] -= Ab[i, j] * x[j];
                if (Math.Abs(Ab[i, i]) > 1e-10)
                    x[i] /= Ab[i, i];
            }

            return x;
        }
    }
}
