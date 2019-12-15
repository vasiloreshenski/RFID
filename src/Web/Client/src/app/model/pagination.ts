export class Pagination<T> {
    public static defaultPage = 0;
    public static defaultPageSize = 5;
    
    public page: number;
    public pageSize: number;
    public pagesCount: number;
    public count: number;
    public items: T[];

    public static empty<T>() {
        const result = new Pagination<T>();
        result.items = [];
        return result;
    }
}

