using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace pokercsharp.mainsource.cfrplus {
    public class KuhnPoker {
        private const int NUM_PLAYERS = 2;
        private readonly int[] plrs = new int[3] { 0, 1, -1 };
        private readonly int[] deck = new int[3] { 0, 1, 2 };
        private Node root;
        private Dictionary<string, List<Node>> information_set = new Dictionary<string, List<Node>>();
        private int next_player;
        private List<Node> stack = new List<Node>();

        public KuhnPoker() {
            BuildGameTree();
            Debug.WriteLine("Init complete");
        }

        private void BuildGameTree() {
            this.next_player = -1;
            this.root = new Node(next_player, false);
            AddToInformationDict(root.GetInformation(), root);
            foreach(int hand_0 in deck) {
                foreach(int hand_1 in deck) {
                    if(hand_0 == hand_1) {
                        continue;
                    }
                    root.SetCard(new int[] { hand_0, hand_1 });
                    next_player = 0;
                    Node node = ExpandAndAddInfoNode(root, "");
                    foreach(string action_0 in new string[]{ "x", "b" }){     //p0 action
                        next_player = 1;
                        Node node_1 = ExpandAndAddInfoNode(node, action_0);
                        if (action_0.Equals("x")) {
                            foreach (string action_1 in new string[] { "x", "b" }) {
                                if (action_1.Equals("x")) {
                                    double profit = this.CalcShowDownProfit(hand_0, hand_1, "x", 1);
                                    next_player = -1;
                                    Node node_2 = ExpandAndAddTerminalNode(node_1, action_1, profit);
                                }
                                if (action_1.Equals("b")) {
                                    next_player = 0;
                                    Node node_2 = ExpandAndAddInfoNode(node_1, action_1);
                                    foreach(string action_2 in new string[] {"c", "f" }) {
                                        double profit = CalcShowDownProfit(hand_0, hand_1, action_2, 0);
                                        next_player = -1;
                                        Node node_3 = ExpandAndAddTerminalNode(node_2, action_2, profit);
                                    }
                                }
                            }
                        }
                        if (action_0.Equals("b")) {
                            foreach(string action_1 in new string[] {"c", "f" }) {
                                double profit = CalcShowDownProfit(hand_0, hand_1, action_1, 1);
                                next_player = -1;
                                Node node_2 = ExpandAndAddTerminalNode(node_1, action_1, profit);
                            }
                        }
                    }
                }
            }
        }

        private double CalcShowDownProfit(int hand_0, int hand_1, string action, int player) {
            bool isHand0Win = hand_0 > hand_1;
            if(action == "f") {
                return player == 0 ? -1 : 1;
            }else if(action == "x") {
                return isHand0Win ? 1 : -1;
            }else if(action == "c") {
                return isHand0Win ? 2 : -2;
            }
            return 0;
        }

        private Node ExpandAndAddInfoNode(Node parentNode, string action) {
            Node node = parentNode.ExpandChildNode(action, next_player, false);
            AddToInformationDict(node.GetInformation(), node);
            stack.Add(node);
            return node;
        }

        private Node ExpandAndAddTerminalNode(Node parentNode, string action, double profit) {
            Node node = parentNode.ExpandChildNode(action, next_player, true, profit);
            AddToInformationDict(node.GetInformation(), node);
            stack.Add(node);
            return node;
        }

        private void AddToInformationDict(string informationKey, Node node) {
            AddToInformationDict(this.next_player, informationKey, node);
        }

        private void AddToInformationDict(int player, string informationKey, Node node) {
            string key = player + "/" + informationKey;
            if (!information_set.ContainsKey(key)) {
                information_set[key] = new List<Node>();
            }
            information_set[key].Add(node);
        }
    }
}
