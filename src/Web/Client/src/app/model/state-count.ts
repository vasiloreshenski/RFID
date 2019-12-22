export class StateCount {
    public active: number;
    public inActive: number;
    public deleted: number;
    public unknown: number;

    public static empty() {
        const obj = new StateCount();
        obj.active = 0;
        obj.inActive = 0;
        obj.unknown = 0;
        obj.deleted = 0;
        return obj;
    }
}
