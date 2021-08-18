using System;
using System.Collections.Generic;
using System.Text;

namespace pokercsharp.mainsource.cfrplus {
    public class CFR {
        public void Train() {

        }

        private void UpdateReachPossibility(Node node, double[] rp_mi_list, double[] rp_i_list) {
            node.reach_possibility = rp_mi_list[node.player] * rp_i_list[node.player];
            node.rp_mi = rp_mi_list[node.player];
            node.rp_i = rp_i_list[node.player];
            if (!node.isTerminal) {
                foreach(Node n in node.children.Values){

                }
            }
        }
    }
}
