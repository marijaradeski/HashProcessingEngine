using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Worker.Options;
public class WorkerOptions
{
    public const string SectionName = "Worker";
    public int WorkerCount { get; set; }
}
