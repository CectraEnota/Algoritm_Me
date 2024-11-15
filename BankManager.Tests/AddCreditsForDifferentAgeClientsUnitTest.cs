namespace bankManager.Tests;

public class AddCreditsForDifferentAgeClientsUnitTest
{
    [Fact]
    public void CreditForMinorClient()
    {
        var client = new Client("Васильев Василий Васильевич", Gender.Man, new DateTime(2010, 12, 24), [JobType.Student], "Нижний Новгород");
        var sbank = new Bank("СБанк");
        sbank.IssueCreditTo(client, 1, 100000);
        Assert.Equal("Кредитная история для Васильев Василий Васильевич:\nНичего не найдено.", client.GetCreditHistory());

    }

    [Fact]
    public void CreditForMinorClientWithBirthdayInThisYear()
    {
        var client = new Client("Васильев Василий Васильевич", Gender.Man, new DateTime(2006, 12, 24), [JobType.Student], "Нижний Новгород");
        var sbank = new Bank("СБанк");
        sbank.IssueCreditTo(client, 1, 100000);
        Assert.Equal("Кредитная история для Васильев Василий Васильевич:\nНичего не найдено.", client.GetCreditHistory());

    }

    [Fact]
    public void CreditForClientWithBirthdayInThisYear()
    {
        var client = new Client("Васильев Василий Васильевич", Gender.Man, new DateTime(2006, 11, 15), [JobType.Student], "Нижний Новгород");
        var sbank = new Bank("СБанк");
        sbank.IssueCreditTo(client, 1, 100000);
        Assert.Equal("Кредитная история для Васильев Василий Васильевич:\n\tкредит в банке СБанк под 19% годовых на сумму 100000 рублей до 15.11.2025\n", client.GetCreditHistory());

    }

    [Fact]
    public void CreditForPensionerClient()
    {
        var client = new Client("Александрова Александра Александровна", Gender.Woman, new DateTime(1964, 11, 15), [JobType.Teacher], "Нижний Новгород");
        var sbank = new Bank("СБанк");
        sbank.IssueCreditTo(client, 1, 100000);
        Assert.Equal("Кредитная история для Александрова Александра Александровна:\n\tкредит в банке СБанк под 19% годовых на сумму 100000 рублей до 15.11.2025\n", client.GetCreditHistory());

    }

}
