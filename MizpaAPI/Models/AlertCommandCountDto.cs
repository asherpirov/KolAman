using System;

namespace MizpaAPI.Models;

public class AlertCommandCountDto
{
    public string Command {get; set;} = string.Empty;
    public int AlertsCount {get;set;}
}
