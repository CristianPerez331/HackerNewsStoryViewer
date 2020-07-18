import { NewsStoryItem } from "./news-story-item.model";
import { Deserializable } from "../interfaces/deserializable.interface";

export class NewsStoryData implements Deserializable {
    totalResultCount: number;
    totalPages: number;
    data: Array<NewsStoryItem>;
    hasError: boolean;

    deserialize(object: any): this {
        Object.assign(this, object);
        this.data = object.data.map(x => new NewsStoryItem().deserialize(x));
        return this;
    }
}