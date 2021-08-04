namespace mainsource.system.handvalue;

public class FinalHand : Comparable<FinalHand> {

    private sealed HandName handName;
    private sealed OptionValue optionValue;

    public FinalHand(HandName handName, OptionValue optionValue){
        this.handName = handName;
        this.optionValue = optionValue;
    }

    public HandName getHandName() {
        return handName;
    }

    public OptionValue getOptionValue() {
        return optionValue;
    }

    public override string toString() {
        return this.handName.name() + ", " + this.optionValue.getDetail(this.handName);
    }

    public override int compareTo(FinalHand o) {
        if(this.handName.getValue() > o.handName.getValue()){
            return 1;
        }
        if(this.handName.getValue() < o.handName.getValue()){
            return -1;
        }
        if(this.optionValue.getValue(this.handName) > o.optionValue.getValue(o.handName)){
            return 1;
        }
        if(this.optionValue.getValue(this.handName) < o.optionValue.getValue(o.handName)){
            return -1;
        }
        return 0;
    }
}