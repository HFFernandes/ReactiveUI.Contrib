using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReactiveUI.Contrib.TestClasses {
    public class ValueItem: ReactiveObject {
        private double _value;

        public double Value {
            get { return _value; }
            set { this.RaiseAndSetIfChanged(x => x.Value, ref _value, value); }
        }
    }
}
