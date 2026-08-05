using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Application.Messages;
public class HashBatchMessage
{
    public List<HashMessage> Hashes { get; set; } = new();
}
