namespace bankManager.Tests;

public class AddCreditsToDifferentBanksUnitTest
{
    private readonly Client _client = new("Васильев Василий Васильевич", Gender.Man, new DateTime(2004, 12, 24), [JobType.Student], "Нижний Новгород");
    private readonly Bank _sbank = new("СБанк");
    private readonly Bank _ibank = new("ЯБанк");

    [Fact]
    public void OneCreditInOneBankTest()
    {
        _sbank.IssueCreditTo(_client, 1, 100000);
        var result = $"Кредитная история для Васильев Василий Васильевич:\n"
                   + $"\tкредит в банке СБанк под 19% годовых на сумму 100000 рублей до {DateTime.Now.AddYears(1):dd.MM.yyyy}\n";
        Check(result);

    }

    [Fact]
    public void TwoCreditsInTwoBanksTest()
    {
        _sbank.IssueCreditTo(_client, 1, 100000);
        _ibank.IssueCreditTo(_client, 1, 10000);
        var result = "Кредитная история для Васильев Василий Васильевич:\n"
                   + $"\tкредит в банке СБанк под 19% годовых на сумму 100000 рублей до {DateTime.Now.AddYears(1):dd.MM.yyyy}\n"
                   + $"\tкредит в банке ЯБанк под 21% годовых на сумму 10000 рублей до {DateTime.Now.AddYears(1):dd.MM.yyyy}\n";
        Check(result);

    }

    [Fact]
    public void TwoCreditsInOneBankTest()
    {
        _sbank.IssueCreditTo(_client, 10, 100000);
        _sbank.IssueCreditTo(_client, 1, 10000);
        var result = "Кредитная история для Васильев Василий Васильевич:\n"
                   + $"\tкредит в банке СБанк под 17% годовых на сумму 100000 рублей до {DateTime.Now.AddYears(10):dd.MM.yyyy}\n"
                   + $"\tкредит в банке СБанк под 20% годовых на сумму 10000 рублей до {DateTime.Now.AddYears(1):dd.MM.yyyy}\n";
        Check(result);

    }

    [Fact]
    public void CreditToBadHistoryClient()
    {
        _ibank.AddFamousCredit(new Credit(_ibank, 17, 100000, new DateTime(2023, 11, 14)), _client);
        _sbank.IssueCreditTo(_client, 1, 10000);
        var result = "Кредитная история для Васильев Василий Васильевич:\n"
                   + $"\tкредит в банке ЯБанк под 17% годовых на сумму 100000 рублей до {new DateTime(2023, 11, 14):dd.MM.yyyy}\n"
                   + $"\tкредит в банке СБанк под 22% годовых на сумму 10000 рублей до {DateTime.Now.AddYears(1):dd.MM.yyyy}\n";
        Check(result);

    }

    private void Check(string result) => Assert.Equal(result, _client.GetCreditHistory());

}

