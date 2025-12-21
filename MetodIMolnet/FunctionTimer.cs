using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MetodIMolnet
{
    public class FunctionTimer
    {
        private readonly ILogger _logger;

        public FunctionTimer(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<FunctionTimer>();
        }

        [Function("TimerFunction")]
        public void Run([TimerTrigger("0 * * * * *")] TimerInfo myTimer)
        {
            //_logger.LogInformation($"[00s] Timer Trigger körd: {DateTime.Now}");
            //_logger.LogInformation(" Timer Trigger körd!");
            //_logger.LogInformation("Här är en snabbguide till CRON-syntaxen i Azure Functions:");

            //string cronTable = @"
            //| Position | Fält       | Exempel | Förklaring               |
            //|----------|------------|---------|---------------------------|
            //| 1        | Sekunder   | 0       | (0–59)                    |
            //| 2        | Minuter    | *       | (0–59) varje minut        |
            //| 3        | Timmar     | */2     | (0–23) varannan timme     |
            //| 4        | Dag i mån  | *       | (1–31) varje dag          |
            //| 5        | Månad      | *       | (1–12) varje månad        |
            //| 6        | Veckodag   | 1-5     | (0–6) Mån–Fre             |
            //";
            
            //_logger.LogError(cronTable);
            //_logger.LogInformation("Exempel: '0 */5 * * * *' = Var 5:e minut");
        }
    }
}