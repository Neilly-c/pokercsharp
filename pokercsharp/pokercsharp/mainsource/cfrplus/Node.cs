using System;
using System.Collections.Generic;
using System.Text;

namespace pokercsharp.mainsource.cfrplus {
    public class Node {
        public Dictionary<string, Node> children = new Dictionary<string, Node>();
        public int player;
        public bool isTerminal = false;
        private int[] cards = new int[2];
        public string history = "";
        public string information = "";

        public double reach_possibility;
        public double rp_mi, rp_i;
        private double expected_profit;
        private double counterfactual_value;
        private Dictionary<string, double> counterfactual_regret = new Dictionary<string, double>();
        private double pi_i_sum;
        private Dictionary<string, double> pi_sigma_sum = new Dictionary<string, double>();

        public Node(int next_player, bool isTerminal) {
            this.player = next_player;
            this.isTerminal = isTerminal;
        }

        public Node(int next_player, bool isTerminal, double profit) : this(next_player, isTerminal) {
            this.expected_profit = profit;
        }

        public Node ExpandChildNode(string action, int next_player, bool isTerminal) {
            Node next_node = new Node(next_player, isTerminal);
            next_node.cards = this.cards;
            next_node.history = this.history;
            if(this.player != -1) {
                next_node.history += action;    //これってやっていいの？
            }
            next_node.isTerminal = isTerminal;
            next_node.information = next_player == -1 ? next_node.history : cards[next_player] + next_node.history;

            string key = string.Join("", next_node.cards) + action;
            this.children.Add(key, next_node);
            this.counterfactual_regret.Add(key, 0);
            this.pi_sigma_sum.Add(key, 0);

            return next_node;
        }

        public Node ExpandChildNode(string action, int next_player, bool isTerminal, double profit) {
            Node next_node = ExpandChildNode(action, next_player, isTerminal);
            next_node.expected_profit = profit;

            return next_node;
        }

        public void SetCard(int[] cards) {
            this.cards = cards;
        }

        public string GetInformation() {
            return this.information;
        }

    }
}
