using pokercsharp.mainsource.cfrplus;
using System;
using System.Collections.Generic;
using System.Text;

namespace pokercsharp.mainsource.system {
    public class WorkerThread {

        MainWindow callFrom { get; }
        public CFRBase cfr { get; set; }
        public double stack { get; set; }
        public int iter { get; set; }
        public string board { get; set; }
        public bool isToCalcPostflop { get; set; }
        Dictionary<string, Node> nodeMap;

        public WorkerThread(MainWindow callFrom, CFRBase cfr, double stack, int iter, string board, bool isToCalcPostflop) {
            this.callFrom = callFrom;
            this.cfr = cfr;
            this.stack = stack;
            this.iter = iter;
            this.board = board;
            this.isToCalcPostflop = isToCalcPostflop;
        }

        public void ThreadASync() {
            for (int i = 0; i < (iter >= 6 ? Math.Pow(10, iter - 6) : 1); ++i) {
                if (isToCalcPostflop) {
                    nodeMap = cfr.Run(1, board, (int)stack, 2);
                    callFrom.Dispatcher.Invoke((Action)(() => {
                        Console.WriteLine("iteration " + i);
                    }));
                } else {
                    nodeMap = cfr.Run(10000, (int)stack);
                    callFrom.Dispatcher.Invoke((Action)(() => {
                        Console.WriteLine("iteration " + i);
                    }));
                }
            }
            callFrom.Dispatcher.Invoke((Action)(() => {
                callFrom.nodeMap = nodeMap;
            }));

        }
    }
}
