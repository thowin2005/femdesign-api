﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FemDesign.Foundations
{
    public partial class Foundations
    {
        [XmlElement("isolated_foundation")]
        public List<IsolatedFoundation> IsolatedFoundations = new List<IsolatedFoundation>();

        [XmlElement("wall_foundation")]
        private List<StruSoft.Interop.StruXml.Data.Lnfoundation_type> wall_foundationField = new List<StruSoft.Interop.StruXml.Data.Lnfoundation_type>();

        [XmlElement("foundation_slab")]
        private List<StruSoft.Interop.StruXml.Data.Sffoundation_type> foundation_slabField = new List<StruSoft.Interop.StruXml.Data.Sffoundation_type>();
    }
}
