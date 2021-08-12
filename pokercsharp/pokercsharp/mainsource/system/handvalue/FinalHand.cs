using System;

namespace mainsource.system.handvalue {

    public class FinalHand : IComparable<FinalHand> {

        private readonly HandName handName;
        private readonly OptionValue optionValue;

        public FinalHand(HandName handName, OptionValue optionValue) {
            this.handName = handName;
            this.optionValue = optionValue;
        }

        public HandName GetHandName() {
            return handName;
        }

        public OptionValue GetOptionValue() {
            return optionValue;
        }

        public override int GetHashCode() {
            return (((int)handName) << 20) + optionValue.GetHashCode();
        }

        public override string ToString() {
            return this.handName.ToString() + ", " + this.optionValue.GetDetail(this.handName);
        }

        public int CompareTo(FinalHand o) {
            if (this.handName.GetValue() > o.handName.GetValue()) {
                return 1;
            }
            if (this.handName.GetValue() < o.handName.GetValue()) {
                return -1;
            }
            if (this.optionValue.GetValue(this.handName) > o.optionValue.GetValue(o.handName)) {
                return 1;
            }
            if (this.optionValue.GetValue(this.handName) < o.optionValue.GetValue(o.handName)) {
                return -1;
            }
            return 0;
        }
    }
}
