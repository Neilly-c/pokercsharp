using System;
using System.Collections.Generic;
using System.Text;

namespace pokercsharp.mainsource.cfrplus {
    public class NodeGroup {
        public string history { get; set; }
        public List<NodeGroup> children { get; set; } = new List<NodeGroup>();

        public NodeGroup(string history) {
            this.history = history;
        }
    }
}
