using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("bankManager.Tests")]
namespace bankManager;

public class Bank(string name)
{
    private readonly string _name = name;
    private readonly List<Client> _clients = [];
    private readonly static Dictionary<Credit, Client> _credits = [];

    public static double KeyRate => 21;
    public string Name => _name;

    public void RegisteredNewClient(Client client)
    {
        _clients.Add(client);

    }

    internal void AddFamousCredit(Credit credit, Client client) => _credits.Add(credit, client);

    public void IssueCreditTo(Client client, int countYears, int amount)
    {
        var nowTime = DateTime.Now;
        var deadline = nowTime.AddYears(countYears);
        var rate = KeyRate;
        List<Credit> clientCredits = [];
        bool isFailDeadline = false;
        bool isThisBankClient = false;

        //Проверяем на совершеннолетие
        var clientYears = nowTime.Year - client.BirthDate.Year;
        if (clientYears < 18) return;
        if (clientYears == 18)
        {
            if (client.BirthDate.Month > nowTime.Month) return;
            if (client.BirthDate.Month == nowTime.Month)
            {
                if (client.BirthDate.Day > nowTime.Day) return;

            }

        }

        //В зависимости от срока, на который даём кредит, меняем ставку
        if (deadline.Year <= 1) rate++;
        if (deadline.Year <= 5) rate++;
        if (deadline.Year >= 10) rate--;

        //В зависимости от суммы займа меням ставку
        if (amount <= 10000) rate++;
        if (amount <= 100000) rate++;
        if (amount >= 1000000) rate--;

        //Проверяем данные клиента для подбора условий кредита
        if (client.City != "Москва" && client.City != "Санкт-Петербург") rate--;
        if (client.JobsType.Contains(JobType.Student)) rate--;
        if (client.JobsType.Contains(JobType.Programmer)) rate--;

        //Проверка, является ли клиент пенсионером
        if (client.Gender == Gender.Man)
        {
            if (clientYears < 65) rate--;
            else if (clientYears == 65)
            {
                if (client.BirthDate.Month < nowTime.Month) rate--;
                else if (client.BirthDate.Month == nowTime.Month)
                {
                    if (client.BirthDate.Day < nowTime.Day) rate--;

                }

            }

        }
        else
        {
            if (clientYears < 60) rate--;
            else if (clientYears == 60)
            {
                if (client.BirthDate.Month < nowTime.Month) rate--;
                else if (client.BirthDate.Month == nowTime.Month)
                {
                    if (client.BirthDate.Day < nowTime.Day) rate--;

                }

            }

        }

        //Проверяем кредитную историю клиента

        //Если кредитов нет, понижаем ставку
        if (!_credits.ContainsValue(client)) rate--;
        
        //Находим кредиты клиента
        foreach (var creditPair in _credits)
        {
            if (creditPair.Value == client)
            {
                var credit = creditPair.Key;
                //Проверяем, просрочен ли кредит
                if (credit.RepaymentDate < nowTime) isFailDeadline = true;
                //Проверяем, был ли взят кредит в этом банке
                if (credit.Bank == this) isThisBankClient = true;

            }

        }

        if (isFailDeadline) rate++;
        if (isThisBankClient) rate--;

        Credit newCredit = new Credit(this, rate, amount, deadline);
        _credits.Add(newCredit, client);

    }

    public static List<Credit> GetAllCreditsFor(Client client) 
        => _credits.Where(creditPair => creditPair.Value == client)
                   .Select(creditPair => creditPair.Key)
                   .ToList();

}
