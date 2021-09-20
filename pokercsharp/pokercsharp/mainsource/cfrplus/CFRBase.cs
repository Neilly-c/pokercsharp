using mainsource.system.card;
using System;
using System.Collections.Generic;
using System.Text;

namespace pokercsharp.mainsource.cfrplus {
    public class CFRBase {

        protected const char FOLD = 'f', CHECK = 'x', CALL = 'c', RAISE_A = 'r', PUSH = 'p';
        protected const double RAISE_A_RATIO = 0.5d;
        protected Card[] cards;
        public Dictionary<string, Node> nodeMap { get; set; } = new Dictionary<string, Node>();
        public double utilPerIterations { get; set; }
        protected double stack = 2, bb = 1, sb = 0.5d, ante = 0;

        public virtual Dictionary<string, Node> Run(int iter, int stack) {
            return null;
        }

        public virtual Dictionary<string, Node> Run(int iter, string card_str, int pot, int stack) {
            return null;
        }

        public virtual NodeGroup CreateNode(ref List<NodeGroup> list, string history, double stack, double pot, double toCall) {
            return null;
        }

        public void RefreshNodeMap() {
            nodeMap.Clear();
        }
    }
}
