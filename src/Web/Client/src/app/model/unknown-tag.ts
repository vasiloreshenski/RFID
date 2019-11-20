export class UnknownTag {
    public id: number;
    public accessDate: Date;
    public number: string;

    public getAccessDateDisplayText(): string {
        return this.accessDate.toLocaleString();
    }
}
