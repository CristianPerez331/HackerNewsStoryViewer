export interface Deserializable {
    deserialize(object: any): this;
}