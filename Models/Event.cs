using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Event
{
    public string? IdEvent { get; set; }

    public string? NameEvent { get; set; }

    public string? Date { get; set; }

    public string? Location { get; set; }

    public string? LinkToRegistration { get; set; }

    public string? Description { get; set; }

    public string? AmountPeople { get; set; }

    public string? RegulationId { get; set; }
}
