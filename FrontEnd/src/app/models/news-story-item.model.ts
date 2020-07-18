import { Deserializable } from "../interfaces/deserializable.interface";

export class NewsStoryItem implements Deserializable {
    id: number;
    author: string;
    title: string;
    url: string;
    timePublished: number;

    deserialize(object: any) {
        Object.assign(this, object);
        return this;
    }
}