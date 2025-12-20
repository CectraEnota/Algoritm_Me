namespace AlgMe1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components1 = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components1 != null))
            {
                components1.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpCircuit = new System.Windows.Forms.GroupBox();
            this.lblHelp = new System.Windows.Forms.Label();
            this.lblCircuitInput = new System.Windows.Forms.Label();
            this.txtCircuitDescription = new System.Windows.Forms.TextBox();
            this.grpSimParams = new System.Windows.Forms.GroupBox();
            this.lblTotalTime = new System.Windows.Forms.Label();
            this.txtTotalTime = new System.Windows.Forms.TextBox();
            this.lblTimeStep = new System.Windows.Forms.Label();
            this.txtTimeStep = new System.Windows.Forms.TextBox();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.btnLoadExample = new System.Windows.Forms.Button();
            this.btnParseCircuit = new System.Windows.Forms.Button();
            this.btnBuildGraph = new System.Windows.Forms.Button();
            this.btnBuildEquations = new System.Windows.Forms.Button();
            this.btnSimulate = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.grpInitialConditions = new System.Windows.Forms.GroupBox();
            this.lblICHelp = new System.Windows.Forms.Label();
            this.rbICFieldsMode = new System.Windows.Forms.RadioButton();
            this.rbICTextMode = new System.Windows.Forms.RadioButton();
            this.lblInitialConditions = new System.Windows.Forms.Label();
            this.panelInitialConditions = new System.Windows.Forms.Panel();
            this.txtInitialConditionsText = new System.Windows.Forms.TextBox();
            this.btnResetIC = new System.Windows.Forms.Button();
            this.lblLog = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lblGraph = new System.Windows.Forms.Label();
            this.panelGraph = new System.Windows.Forms.Panel();
            this.lblResults = new System.Windows.Forms.Label();
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.grpCircuit.SuspendLayout();
            this.grpSimParams.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.grpInitialConditions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            this.SuspendLayout();
            // 
            // grpCircuit
            // 
            this.grpCircuit.Controls.Add(this.lblHelp);
            this.grpCircuit.Controls.Add(this.lblCircuitInput);
            this.grpCircuit.Controls.Add(this.txtCircuitDescription);
            this.grpCircuit.Location = new System.Drawing.Point(12, 12);
            this.grpCircuit.Name = "grpCircuit";
            this.grpCircuit.Size = new System.Drawing.Size(380, 400);
            this.grpCircuit.TabIndex = 0;
            this.grpCircuit.TabStop = false;
            this.grpCircuit.Text = "1. Описание схемы";
            // 
            // lblCircuitInput
            // 
            this.lblCircuitInput.AutoSize = true;
            this.lblCircuitInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblCircuitInput.Location = new System.Drawing.Point(6, 20);
            this.lblCircuitInput.Name = "lblCircuitInput";
            this.lblCircuitInput.Size = new System.Drawing.Size(245, 15);
            this.lblCircuitInput.TabIndex = 0;
            this.lblCircuitInput.Text = "Введите компоненты схемы:";
            // 
            // lblHelp
            // 
            this.lblHelp.AutoSize = false;
            this.lblHelp.ForeColor = System.Drawing.Color.Gray;
            this.lblHelp.Location = new System.Drawing.Point(6, 38);
            this.lblHelp.Name = "lblHelp";
            this.lblHelp.Size = new System.Drawing.Size(365, 150);
            this.lblHelp.TabIndex = 1;
            this.lblHelp.Text =
                                "Формат: ТИП ИМЯ УЗЕЛ1 УЗЕЛ2 ЗНАЧЕНИЕ\r\n\r\n" +
                                "R - резистор (Ом)\r\n" +
                                "L - индуктивность (Гн)\r\n" +
                                "C - конденсатор (Ф)\r\n" +
                                "V - источник напряжения (В)\r\n" +
                                "I - источник тока (А)\r\n" +
                                "VCCS - ИТУН (См): VCCS имя n+ n- nc+ nc- S\r\n\r\n" +
                                "Пример: C C1 1 2 0.00001\r\n" +
                                "VCCS: VCCS S1 2 0 1 0 0.001";
            // 
            // txtCircuitDescription
            // 
            this.txtCircuitDescription.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtCircuitDescription.Location = new System.Drawing.Point(9, 200);
            this.txtCircuitDescription.Multiline = true;
            this.txtCircuitDescription.Name = "txtCircuitDescription";
            this.txtCircuitDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCircuitDescription.Size = new System.Drawing.Size(365, 190);
            this.txtCircuitDescription.TabIndex = 2;
            this.txtCircuitDescription.Text =
                                "// Пример 1: Схема из лекции\r\n" +
                                "R R1 1 2 100\r\n" +
                                "R R2 0 1 200\r\n" +
                                "L L1 2 0 0.001\r\n" +
                                "C C1 1 0 0.00001\r\n" +
                                "I J1 0 1 0.01\r\n\r\n" +
                                "// Пример 2: МДП-инвертор\r\n" +
                                "// R R1 1 2 10000\r\n" +
                                "// C Csi 2 0 1e-9\r\n" +
                                "// VCCS S1 2 0 1 0 0.001\r\n" +
                                "// V Vdd 1 0 5\r\n" +
                                "// V Vin 1 0 0";
            // 
            // grpSimParams
            // 
            this.grpSimParams.Controls.Add(this.lblTotalTime);
            this.grpSimParams.Controls.Add(this.txtTotalTime);
            this.grpSimParams.Controls.Add(this.lblTimeStep);
            this.grpSimParams.Controls.Add(this.txtTimeStep);
            this.grpSimParams.Location = new System.Drawing.Point(12, 368);
            this.grpSimParams.Name = "grpSimParams";
            this.grpSimParams.Size = new System.Drawing.Size(380, 100);
            this.grpSimParams.TabIndex = 1;
            this.grpSimParams.TabStop = false;
            this.grpSimParams.Text = "2. Параметры симуляции";
            // 
            // lblTotalTime
            // 
            this.lblTotalTime.AutoSize = true;
            this.lblTotalTime.Location = new System.Drawing.Point(15, 30);
            this.lblTotalTime.Name = "lblTotalTime";
            this.lblTotalTime.Size = new System.Drawing.Size(150, 13);
            this.lblTotalTime.TabIndex = 0;
            this.lblTotalTime.Text = "Время моделирования (с):";
            // 
            // txtTotalTime
            // 
            this.txtTotalTime.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtTotalTime.Location = new System.Drawing.Point(200, 27);
            this.txtTotalTime.Name = "txtTotalTime";
            this.txtTotalTime.Size = new System.Drawing.Size(150, 23);
            this.txtTotalTime.TabIndex = 1;
            this.txtTotalTime.Text = "0.001";
            // 
            // lblTimeStep
            // 
            this.lblTimeStep.AutoSize = true;
            this.lblTimeStep.Location = new System.Drawing.Point(15, 60);
            this.lblTimeStep.Name = "lblTimeStep";
            this.lblTimeStep.Size = new System.Drawing.Size(150, 13);
            this.lblTimeStep.TabIndex = 2;
            this.lblTimeStep.Text = "Шаг интегрирования (с):";
            // 
            // txtTimeStep
            // 
            this.txtTimeStep.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtTimeStep.Location = new System.Drawing.Point(200, 57);
            this.txtTimeStep.Name = "txtTimeStep";
            this.txtTimeStep.Size = new System.Drawing.Size(150, 23);
            this.txtTimeStep.TabIndex = 3;
            this.txtTimeStep.Text = "0.000001";
            // 
            // grpActions
            // 
            this.grpActions.Controls.Add(this.btnLoadExample);
            this.grpActions.Controls.Add(this.btnParseCircuit);
            this.grpActions.Controls.Add(this.btnBuildGraph);
            this.grpActions.Controls.Add(this.btnBuildEquations);
            this.grpActions.Controls.Add(this.btnSimulate);
            this.grpActions.Controls.Add(this.btnClear);
            this.grpActions.Location = new System.Drawing.Point(12, 474);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(380, 180);
            this.grpActions.TabIndex = 2;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "3. Действия";
            // 
            // btnLoadExample
            // 
            this.btnLoadExample.BackColor = System.Drawing.Color.LightCyan;
            this.btnLoadExample.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnLoadExample.Location = new System.Drawing.Point(15, 25);
            this.btnLoadExample.Name = "btnLoadExample";
            this.btnLoadExample.Size = new System.Drawing.Size(170, 35);
            this.btnLoadExample.TabIndex = 0;
            this.btnLoadExample.Text = "📋 Загрузить пример";
            this.btnLoadExample.UseVisualStyleBackColor = false;
            this.btnLoadExample.Click += new System.EventHandler(this.btnLoadExample_Click);
            // 
            // btnParseCircuit
            // 
            this.btnParseCircuit.BackColor = System.Drawing.Color.LightGreen;
            this.btnParseCircuit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnParseCircuit.Location = new System.Drawing.Point(195, 25);
            this.btnParseCircuit.Name = "btnParseCircuit";
            this.btnParseCircuit.Size = new System.Drawing.Size(170, 35);
            this.btnParseCircuit.TabIndex = 1;
            this.btnParseCircuit.Text = "① Разобрать схему";
            this.btnParseCircuit.UseVisualStyleBackColor = false;
            this.btnParseCircuit.Click += new System.EventHandler(this.btnParseCircuit_Click);
            // 
            // btnBuildGraph
            // 
            this.btnBuildGraph.BackColor = System.Drawing.Color.LightGreen;
            this.btnBuildGraph.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnBuildGraph.Location = new System.Drawing.Point(15, 70);
            this.btnBuildGraph.Name = "btnBuildGraph";
            this.btnBuildGraph.Size = new System.Drawing.Size(170, 35);
            this.btnBuildGraph.TabIndex = 2;
            this.btnBuildGraph.Text = "② Построить граф";
            this.btnBuildGraph.UseVisualStyleBackColor = false;
            this.btnBuildGraph.Click += new System.EventHandler(this.btnBuildGraph_Click);
            // 
            // btnBuildEquations
            // 
            this.btnBuildEquations.BackColor = System.Drawing.Color.LightGreen;
            this.btnBuildEquations.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnBuildEquations.Location = new System.Drawing.Point(195, 70);
            this.btnBuildEquations.Name = "btnBuildEquations";
            this.btnBuildEquations.Size = new System.Drawing.Size(170, 35);
            this.btnBuildEquations.TabIndex = 3;
            this.btnBuildEquations.Text = "③ Сформировать\r\nуравнения";
            this.btnBuildEquations.UseVisualStyleBackColor = false;
            this.btnBuildEquations.Click += new System.EventHandler(this.btnBuildEquations_Click);
            // 
            // btnSimulate
            // 
            this.btnSimulate.BackColor = System.Drawing.Color.LightSalmon;
            this.btnSimulate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSimulate.Location = new System.Drawing.Point(15, 115);
            this.btnSimulate.Name = "btnSimulate";
            this.btnSimulate.Size = new System.Drawing.Size(170, 50);
            this.btnSimulate.TabIndex = 4;
            this.btnSimulate.Text = "④ СИМУЛИРОВАТЬ ▶";
            this.btnSimulate.UseVisualStyleBackColor = false;
            this.btnSimulate.Click += new System.EventHandler(this.btnSimulate_Click);
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.LightCoral;
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnClear.Location = new System.Drawing.Point(195, 115);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(170, 50);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "🗑 Очистить всё";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // grpInitialConditions
            // 
            this.grpInitialConditions.Controls.Add(this.lblICHelp);
            this.grpInitialConditions.Controls.Add(this.rbICFieldsMode);
            this.grpInitialConditions.Controls.Add(this.rbICTextMode);
            this.grpInitialConditions.Controls.Add(this.lblInitialConditions);
            this.grpInitialConditions.Controls.Add(this.panelInitialConditions);
            this.grpInitialConditions.Controls.Add(this.txtInitialConditionsText);
            this.grpInitialConditions.Controls.Add(this.btnResetIC);
            this.grpInitialConditions.Location = new System.Drawing.Point(12, 660);
            this.grpInitialConditions.Name = "grpInitialConditions";
            this.grpInitialConditions.Size = new System.Drawing.Size(380, 330);
            this.grpInitialConditions.TabIndex = 3;
            this.grpInitialConditions.TabStop = false;
            this.grpInitialConditions.Text = "4. Начальные условия (опционально)";
            this.grpInitialConditions.Visible = false;
            // 
            // lblInitialConditions
            // 
            this.lblInitialConditions.AutoSize = true;
            this.lblInitialConditions.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblInitialConditions.Location = new System.Drawing.Point(10, 20);
            this.lblInitialConditions.Name = "lblInitialConditions";
            this.lblInitialConditions.Size = new System.Drawing.Size(250, 15);
            this.lblInitialConditions.TabIndex = 0;
            this.lblInitialConditions.Text = "Задайте начальные значения:";
            // 
            // rbICFieldsMode
            // 
            this.rbICFieldsMode.AutoSize = true;
            this.rbICFieldsMode.Checked = true;
            this.rbICFieldsMode.Location = new System.Drawing.Point(15, 45);
            this.rbICFieldsMode.Name = "rbICFieldsMode";
            this.rbICFieldsMode.Size = new System.Drawing.Size(130, 17);
            this.rbICFieldsMode.TabIndex = 1;
            this.rbICFieldsMode.TabStop = true;
            this.rbICFieldsMode.Text = "Через поля ввода";
            this.rbICFieldsMode.UseVisualStyleBackColor = true;
            this.rbICFieldsMode.CheckedChanged += new System.EventHandler(this.rbICMode_CheckedChanged);
            // 
            // rbICTextMode
            // 
            this.rbICTextMode.AutoSize = true;
            this.rbICTextMode.Location = new System.Drawing.Point(160, 45);
            this.rbICTextMode.Name = "rbICTextMode";
            this.rbICTextMode.Size = new System.Drawing.Size(130, 17);
            this.rbICTextMode.TabIndex = 2;
            this.rbICTextMode.Text = "Текстовый ввод";
            this.rbICTextMode.UseVisualStyleBackColor = true;
            this.rbICTextMode.CheckedChanged += new System.EventHandler(this.rbICMode_CheckedChanged);
            // 
            // lblICHelp
            // 
            this.lblICHelp.AutoSize = true;
            this.lblICHelp.ForeColor = System.Drawing.Color.Gray;
            this.lblICHelp.Location = new System.Drawing.Point(15, 70);
            this.lblICHelp.Name = "lblICHelp";
            this.lblICHelp.Size = new System.Drawing.Size(350, 26);
            this.lblICHelp.TabIndex = 3;
            this.lblICHelp.Text = "По умолчанию все начальные условия = 0\r\n" +
                                "Формат текстового ввода: IC Uc_C1 5.0";
            // 
            // panelInitialConditions
            // 
            this.panelInitialConditions.AutoScroll = true;
            this.panelInitialConditions.BackColor = System.Drawing.Color.White;
            this.panelInitialConditions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelInitialConditions.Location = new System.Drawing.Point(15, 105);
            this.panelInitialConditions.Name = "panelInitialConditions";
            this.panelInitialConditions.Size = new System.Drawing.Size(350, 180);
            this.panelInitialConditions.TabIndex = 4;
            // 
            // txtInitialConditionsText
            // 
            this.txtInitialConditionsText.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtInitialConditionsText.Location = new System.Drawing.Point(15, 105);
            this.txtInitialConditionsText.Multiline = true;
            this.txtInitialConditionsText.Name = "txtInitialConditionsText";
            this.txtInitialConditionsText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInitialConditionsText.Size = new System.Drawing.Size(350, 180);
            this.txtInitialConditionsText.TabIndex = 5;
            this.txtInitialConditionsText.Text = "// Примеры:\r\n" +
                                                    "// IC Uc_C1 5.0\r\n" +
                                                    "// IC IL_L1 0.1\r\n";
            this.txtInitialConditionsText.Visible = false;
            // 
            // btnResetIC
            // 
            this.btnResetIC.BackColor = System.Drawing.Color.LightYellow;
            this.btnResetIC.Location = new System.Drawing.Point(15, 293);
            this.btnResetIC.Name = "btnResetIC";
            this.btnResetIC.Size = new System.Drawing.Size(150, 30);
            this.btnResetIC.TabIndex = 6;
            this.btnResetIC.Text = "Сбросить в 0";
            this.btnResetIC.UseVisualStyleBackColor = false;
            this.btnResetIC.Click += new System.EventHandler(this.btnResetIC_Click);
            // 
            // lblLog
            // 
            this.lblLog.AutoSize = true;
            this.lblLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblLog.Location = new System.Drawing.Point(12, 1000);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(92, 17);
            this.lblLog.TabIndex = 4;
            this.lblLog.Text = "Лог работы";
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.White;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtLog.Location = new System.Drawing.Point(12, 1020);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(380, 250);
            this.txtLog.TabIndex = 5;
            // 
            // lblGraph
            // 
            this.lblGraph.AutoSize = true;
            this.lblGraph.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblGraph.Location = new System.Drawing.Point(410, 12);
            this.lblGraph.Name = "lblGraph";
            this.lblGraph.Size = new System.Drawing.Size(267, 18);
            this.lblGraph.TabIndex = 6;
            this.lblGraph.Text = "Графики переменных состояния";
            // 
            // panelGraph
            // 
            this.panelGraph.BackColor = System.Drawing.Color.White;
            this.panelGraph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelGraph.Location = new System.Drawing.Point(410, 40);
            this.panelGraph.Name = "panelGraph";
            this.panelGraph.Size = new System.Drawing.Size(1100, 600);
            this.panelGraph.TabIndex = 7;
            this.panelGraph.Paint += new System.Windows.Forms.PaintEventHandler(this.panelGraph_Paint);
            // 
            // lblResults
            // 
            this.lblResults.AutoSize = true;
            this.lblResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblResults.Location = new System.Drawing.Point(410, 650);
            this.lblResults.Name = "lblResults";
            this.lblResults.Size = new System.Drawing.Size(198, 18);
            this.lblResults.TabIndex = 8;
            this.lblResults.Text = "Результаты симуляции";
            // 
            // dgvResults
            // 
            this.dgvResults.AllowUserToAddRows = false;
            this.dgvResults.AllowUserToDeleteRows = false;
            this.dgvResults.BackgroundColor = System.Drawing.Color.White;
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Location = new System.Drawing.Point(410, 675);
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.ReadOnly = true;
            this.dgvResults.RowHeadersWidth = 51;
            this.dgvResults.Size = new System.Drawing.Size(1100, 595);
            this.dgvResults.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1540, 880);
            this.Controls.Add(this.dgvResults);
            this.Controls.Add(this.lblResults);
            this.Controls.Add(this.panelGraph);
            this.Controls.Add(this.lblGraph);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lblLog);
            this.Controls.Add(this.grpInitialConditions);
            this.Controls.Add(this.grpActions);
            this.Controls.Add(this.grpSimParams);
            this.Controls.Add(this.grpCircuit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Универсальный симулятор электрических цепей - Метод переменных состояния";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.grpCircuit.ResumeLayout(false);
            this.grpCircuit.PerformLayout();
            this.grpSimParams.ResumeLayout(false);
            this.grpSimParams.PerformLayout();
            this.grpActions.ResumeLayout(false);
            this.grpInitialConditions.ResumeLayout(false);
            this.grpInitialConditions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox grpCircuit;
        private System.Windows.Forms.Label lblHelp;
        private System.Windows.Forms.Label lblCircuitInput;
        private System.Windows.Forms.TextBox txtCircuitDescription;
        private System.Windows.Forms.GroupBox grpSimParams;
        private System.Windows.Forms.Label lblTotalTime;
        private System.Windows.Forms.TextBox txtTotalTime;
        private System.Windows.Forms.Label lblTimeStep;
        private System.Windows.Forms.TextBox txtTimeStep;
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.Button btnLoadExample;
        private System.Windows.Forms.Button btnParseCircuit;
        private System.Windows.Forms.Button btnBuildGraph;
        private System.Windows.Forms.Button btnBuildEquations;
        private System.Windows.Forms.Button btnSimulate;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.GroupBox grpInitialConditions;
        private System.Windows.Forms.Label lblICHelp;
        private System.Windows.Forms.RadioButton rbICFieldsMode;
        private System.Windows.Forms.RadioButton rbICTextMode;
        private System.Windows.Forms.Label lblInitialConditions;
        private System.Windows.Forms.Panel panelInitialConditions;
        private System.Windows.Forms.TextBox txtInitialConditionsText;
        private System.Windows.Forms.Button btnResetIC;
        private System.Windows.Forms.Label lblLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label lblGraph;
        private System.Windows.Forms.Panel panelGraph;
        private System.Windows.Forms.Label lblResults;
        private System.Windows.Forms.DataGridView dgvResults;
    }
}

