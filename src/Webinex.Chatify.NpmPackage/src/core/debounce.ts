interface DebounceInternalValue<T> {
  value: T;
  enqueueTime: number;
}

export interface DebounceValue<T> extends DebounceInternalValue<T> {
  state: 'enqueued' | 'processing';
}

export interface DebounceConfig<T> {
  timeout: number;
  callback: (value: T) => Promise<any>;
  groupBy: (value: T) => string;
  compare: (a: T, b: T) => number;
}

export class Debounce<T> {
  private _enqueued: DebounceInternalValue<T>[] = [];
  private _processing: DebounceInternalValue<T>[] = [];
  private _timeoutByGroup: Record<string, NodeJS.Timeout> = {};
  private _subscribers: ((value: Debounce<T>) => any)[] = [];

  constructor(public config: DebounceConfig<T>) {}

  public get units(): DebounceValue<T>[] {
    return [
      ...this._enqueued.map((x) => ({ ...x, state: 'enqueued' as const })),
      ...this._processing.map((x) => ({ ...x, state: 'processing' as const })),
    ];
  }

  public subscribe = (callback: (value: Debounce<T>) => any) => {
    this._subscribers.push(callback);
    return () => {
      this._subscribers = this._subscribers.filter((x) => x !== callback);
    };
  };

  public notify = () => {
    this._subscribers.forEach((subscriber) => subscriber(this));
  };

  public push(value: T) {
    const group = this.config.groupBy(value);
    const unit = this._enqueued.find((unit) => this.config.groupBy(unit.value) === group);

    if (!unit) {
      this._enqueued.push({ value, enqueueTime: Date.now() });
    } else if (this.config.compare(unit.value, value) >= 0) {
      unit.value = value;
      unit.enqueueTime = Date.now();
    }

    this._process(group);
  }

  private _process(group: string) {
    if (!this._enqueued.some((unit) => this.config.groupBy(unit.value) === group)) {
      return;
    }

    let timeout = this._timeoutByGroup[group];
    timeout && clearTimeout(timeout);

    if (this._processing.some((unit) => this.config.groupBy(unit.value) === group)) {
      return;
    }

    timeout = setTimeout(() => {
      const processing = this._enqueued.find((unit) => this.config.groupBy(unit.value) === group)!;
      this._processing.push(processing);
      this._enqueued.splice(this._enqueued.indexOf(processing), 1);
      this.notify();

      this.config
        .callback(processing.value)
        .then(() => {
          this._processing.splice(this._processing.indexOf(processing), 1);
          this.notify();
          this._process(group);
        })
        .catch(() => {
          this._processing.splice(this._processing.indexOf(processing), 1);
          this.push(processing.value);
        });
    }, this.config.timeout);

    this._timeoutByGroup[group] = timeout;
  }
}
