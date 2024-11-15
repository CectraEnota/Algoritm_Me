namespace bankManager;

public class Client(string FIO, Gender gender, DateTime birthDate, List<JobType> jobsType, string city)
{
    private readonly string _FIO =  FIO;
    private readonly Gender _gender = gender;
    private readonly DateTime _birthDate = birthDate;
    private readonly string _city = city;
    private readonly List<JobType> _jobsType = jobsType;

    public string FIO => _FIO;
    public Gender Gender => _gender;
    public DateTime BirthDate => _birthDate;
    public string City => _city;
    public IReadOnlyList<JobType> JobsType => _jobsType;

    public string GetCreditHistory()
    {
        string result = $"Кредитная история для {_FIO}:\n";
        var credits = Bank.GetAllCreditsFor(this);

        if (credits.Count == 0) return result + "Ничего не найдено.";
        
        foreach(var credit in credits)
        {
            result += $"\tкредит в банке {credit.Bank.Name} под {credit.Rate}% годовых на сумму {credit.Amount} рублей до {credit.RepaymentDate:dd.MM.yyyy}\n";

        }

        return result;

    }

}
