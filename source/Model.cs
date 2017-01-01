using System.Collections.Generic;
using System.Runtime.Serialization;

public class Model
{
    [DataContract]
    public class Set
    {
        [DataMember]
        public List<Card> cards { get; set; }

        [DataMember]
        public string name { get; set; }
    }

    [DataContract]
    public class Card
    {
        [DataMember]
        public string name { get; set; }
    }
}
