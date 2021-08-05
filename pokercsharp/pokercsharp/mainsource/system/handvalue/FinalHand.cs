namespace mainsource.system.handvalue{

  public class FinalHand : Comparable<FinalHand> {

      private const HandName handName;
      private const OptionValue optionValue;

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

      public string toString() {
          return this.handName.ToString() + ", " + this.optionValue.getDetail(this.handName);
      }

      public int compareTo(FinalHand o) {
          if(this.handName.GetValue() > o.handName.GetValue()){
              return 1;
          }
          if(this.handName.GetValue() < o.handName.GetValue()){
              return -1;
          }
          if(this.optionValue.GetValue(this.handName) > o.optionValue.GetValue(o.handName)){
              return 1;
          }
          if(this.optionValue.GetValue(this.handName) < o.optionValue.GetValue(o.handName)){
              return -1;
          }
          return 0;
      }
  }
}