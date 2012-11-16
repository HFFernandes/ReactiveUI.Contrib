namespace ReactiveUI.Contrib {
    public class KeyedReference<TKey, TValue> : ReactiveObject {
        private TKey _key;
        private TValue _value;

        public KeyedReference() {            
        }
        
        public TKey Key {
            get { return _key; }
            set { this.RaiseAndSetIfChanged(x => x.Key, ref _key, value); }
        }

        public TValue Value {
            get { return _value; }
            set { this.RaiseAndSetIfChanged(x => x.Value, ref _value, value); }
        }
    }
}