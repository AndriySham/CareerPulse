export interface ApplicationCommunicationDto {
  id: string;
  occurredAt: string;
  notes: string;
}

export interface CreateApplicationCommunicationDto {
  occurredAt: string;
  notes: string;
}

export interface UpdateApplicationCommunicationDto {
  occurredAt: string;
  notes: string;
}
