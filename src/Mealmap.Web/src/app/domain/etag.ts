export class ETag {
  private _value: string;

  constructor(value: string) {
    this._value = value;
  }

  toString(): string {
    return this._value;
  }
}
