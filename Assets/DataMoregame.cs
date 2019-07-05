using UnityEngine;
using System.Collections;
using Bitrave.Azure;
using Newtonsoft.Json;
using System.Collections.Generic;
using Linq2Rest.Provider;
using System;

public class moregame
{
    public Guid? Id { get; set; }

    [JsonProperty(PropertyName = "username")]
    public string Username { get; set; }

    [JsonProperty(PropertyName = "linkImage")]
    public string LinkImage { get; set; }

    [JsonProperty(PropertyName = "package")]
    public string Package { get; set; }

    public override string ToString()
    {
        return Id + "," + Username + "," + Package;
    }
}
