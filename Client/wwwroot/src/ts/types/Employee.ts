export enum Gender {
    L,
    P
}

export type Employee = {
    nik: string,
    firstName: string,
    lastName: string,
    email: string,
    phoneNumber: string,
    gender: Gender,
    birthdate: Date,
    hiringDate: Date
}