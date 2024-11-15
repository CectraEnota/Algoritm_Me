using bankManager;

var client = new Client("Васильев Василий Васильевич", Gender.Man, new DateTime(2004, 12, 24), [JobType.Student], "Нижний Новгород");
var sbank = new Bank("СБанк");
sbank.IssueCreditTo(client, 1, 100000);
Console.WriteLine (client.GetCreditHistory());