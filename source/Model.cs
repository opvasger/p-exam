using System.Collections.Generic;
using System.Runtime.Serialization;

public class Model
{
    [DataContract]
    public class Set
    {
        [DataMember]
        public List<Card> Cards { get; set; }

        [DataMember]
        public string Name { get; set; }
    }

    [DataContract]
    public class Card
    {
        [DataMember]
        public string Name { get; set; }
    }
}