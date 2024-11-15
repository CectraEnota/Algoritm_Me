namespace bankManager;

public record Credit(Bank Bank, double Rate, int Amount, DateTime RepaymentDate);
