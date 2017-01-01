using System.Collections.Generic;
using System.Runtime.Serialization;

public class Model
{

    [DataContract]
    public class Set
    {
        [DataMember(Name = "cards")]
        public List<Card> Cards { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    [DataContract]
    public class Card
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "colorIdentity")]
        public List<string> Colors { get; set; }

        [DataMember(Name = "supertypes")]
        public List<string> SuperTypes { get; set; }
    }
}
