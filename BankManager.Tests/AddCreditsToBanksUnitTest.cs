namespace bankManager.Tests;

public class AddCreditsToDifferentBanksUnitTest
{
    [Fact]
    public void OneCreditInOneBankTest()
    {
        var client = new Client("Васильев Василий Васильевич", Gender.Man, new DateTime(2004, 12, 24), [JobType.Student], "Нижний Новгород");
        var sbank = new Bank("СБанк");
        sbank.IssueCreditTo(client, 1, 100000);
        Assert.Equal("Кредитная история для Васильев Василий Васильевич:\n\tкредит в банке СБанк под 19% годовых на сумму 100000 рублей до 15.11.2025\n", client.GetCreditHistory());

    }

    [Fact]
    public void TwoCreditsInTwoBanksTest()
    {
        var client = new Client("Васильев Василий Васильевич", Gender.Man, new DateTime(2004, 12, 24), [JobType.Student], "Нижний Новгород");
        var sbank = new Bank("СБанк");
        var ibank = new Bank("ЯБанк");
        sbank.IssueCreditTo(client, 1, 100000);
        ibank.IssueCreditTo(client, 1, 10000);
        Assert.Equal("Кредитная история для Васильев Василий Васильевич:\n\tкредит в банке СБанк под 19% годовых на сумму 100000 рублей до 15.11.2025\n\tкредит в банке ЯБанк под 21% годовых на сумму 10000 рублей до 15.11.2025\n", client.GetCreditHistory());

    }

    [Fact]
    public void TwoCreditsInOneBankTest()
    {
        var client = new Client("Васильев Василий Васильевич", Gender.Man, new DateTime(2004, 12, 24), [JobType.Student], "Нижний Новгород");
        var sbank = new Bank("СБанк");
        sbank.IssueCreditTo(client, 1, 100000);
        sbank.IssueCreditTo(client, 1, 10000);
        Assert.Equal("Кредитная история для Васильев Василий Васильевич:\n\tкредит в банке СБанк под 19% годовых на сумму 100000 рублей до 15.11.2025\n\tкредит в банке СБанк под 20% годовых на сумму 10000 рублей до 15.11.2025\n", client.GetCreditHistory());

    }

    [Fact]
    public void CreditToBadHistoryClient()
    {
        var client = new Client("Васильев Василий Васильевич", Gender.Man, new DateTime(2004, 12, 24), [JobType.Student], "Нижний Новгород");
        var sbank = new Bank("СБанк");
        var ibank = new Bank("ЯБанк");
        ibank.AddFamousCredit(new Credit(ibank, 17, 100000, new DateTime(2023, 11, 14)), client);
        sbank.IssueCreditTo(client, 1, 10000);
        Assert.Equal("Кредитная история для Васильев Василий Васильевич:\n\tкредит в банке ЯБанк под 17% годовых на сумму 100000 рублей до 14.11.2023\n\tкредит в банке СБанк под 22% годовых на сумму 10000 рублей до 15.11.2025\n", client.GetCreditHistory());

    }

}

