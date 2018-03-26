using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
public class Course : Document
{
    [JsonProperty(PropertyName = "CourseId")]
    public Guid CourseId { get; set; }

    [JsonProperty(PropertyName = "Name")]
    public string Name
    {
        get
        {
            return GetPropertyValue<string>("Name");
        }
        set
        {
            SetPropertyValue("Name", value);
        }
    }

    [JsonProperty(PropertyName = "Sessions")]
    public List<Session> Sessions { get; set; }

    [JsonProperty(PropertyName = "Teacher")]
    public Teacher Teacher { get; set; }

    [JsonProperty(PropertyName = "Students")]
    public List<Student> Students { get; set; }
}