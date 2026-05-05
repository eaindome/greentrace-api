namespace GreenTrace.Server.Features.Analytics;

public record BatchStatusSummary(string status, int count);

public record ValueByDomain(long domainId, string domainName, decimal totalValue, string currency);

public record ActorValueRanking(long actorId, string actorName, decimal totalValue, string currency, int recordCount);

public record StageCompletionRate(long stageId, string stageName, short sequence, int batchCount, double completionRate);

public record ValueOverTime(int year, int month, decimal totalValue, int recordCount);
