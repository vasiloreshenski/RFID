export class AppError extends Error {
    public static Login(error: any) {
        const appError = new AppError('Faild during logging. Please try again later.', error);
        throw appError;
    }

    constructor(public message: string, public origin: any) {
        super(message);
        this.name = 'AppError';
    }
}
