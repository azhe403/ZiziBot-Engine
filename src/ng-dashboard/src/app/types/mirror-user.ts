export interface MirrorUser {
  userId: number;
  expireAt: Date;
}

export interface AddMirrorUserDto {
  userId: number;
  monthDuration: Date;
  additionalNote: string;
}
