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
        Dictionary<string, Node> nodeMap;

        public WorkerThread(MainWindow callFrom, CFRBase cfr, double stack, int iter) {
            this.callFrom = callFrom;
            this.cfr = cfr;
            this.stack = stack;
            this.iter = iter;
        }

        public void ThreadASync() {
            for (int i = 0; i < (iter >= 6 ? Math.Pow(10, iter - 6) : 1); ++i) {
                nodeMap = cfr.Run(10000, (int)stack);
                callFrom.Dispatcher.Invoke((Action)(() => {
                    Console.WriteLine("iteration " + i);
                }));

            }
            callFrom.Dispatcher.Invoke((Action)(() => {
                callFrom.nodeMap = nodeMap;
            }));

        }
    }
}
