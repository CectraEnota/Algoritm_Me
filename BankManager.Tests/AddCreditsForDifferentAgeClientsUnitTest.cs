namespace bankManager.Tests;

public class AddCreditsForDifferentAgeClientsUnitTest
{
    private readonly Bank _sbank = new("СБанк");

    [Fact]
    public void CreditForMinorClient()
    {
        var client = new Client("Васильев Василий Васильевич", Gender.Man, new DateTime(2010, 12, 24), [JobType.Student], "Нижний Новгород");
        _sbank.IssueCreditTo(client, 1, 100000);
        var result = "Кредитная история для Васильев Василий Васильевич:\n"
                   + "Ничего не найдено.";
        Assert.Equal(result, client.GetCreditHistory());

    }

    [Fact]
    public void CreditForMinorClientWithBirthdayInThisYear()
    {
        var client = new Client("Васильев Василий Васильевич", Gender.Man, new DateTime(2006, 12, 24), [JobType.Student], "Нижний Новгород");
        _sbank.IssueCreditTo(client, 1, 100000);
        var result = "Кредитная история для Васильев Василий Васильевич:\n"
                   + "Ничего не найдено.";
        Assert.Equal(result, client.GetCreditHistory());

    }

    [Fact]
    public void CreditForClientWithBirthdayInThisYear()
    {
        var client = new Client("Васильев Василий Васильевич", Gender.Man, new DateTime(2006, 11, 15), [JobType.Student], "Нижний Новгород");
        _sbank.IssueCreditTo(client, 1, 100000);
        var result = "Кредитная история для Васильев Василий Васильевич:\n"
                   + $"\tкредит в банке СБанк под 19% годовых на сумму 100000 рублей до {DateTime.Now.AddYears(1):dd.MM.yyyy}\n";
        Assert.Equal(result, client.GetCreditHistory());

    }

    [Fact]
    public void CreditForPensionerClient()
    {
        var client = new Client("Александрова Александра Александровна", Gender.Woman, new DateTime(1964, 11, 15), [JobType.Teacher], "Нижний Новгород");
        _sbank.IssueCreditTo(client, 1, 100000);
        var result = "Кредитная история для Александрова Александра Александровна:\n"
                   + $"\tкредит в банке СБанк под 19% годовых на сумму 100000 рублей до {DateTime.Now.AddYears(1):dd.MM.yyyy}\n";
        Assert.Equal(result, client.GetCreditHistory());

    }

}
