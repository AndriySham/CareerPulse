export type InterviewType =
  | 'PhoneScreen'
  | 'HRInterview'
  | 'TechnicalInterview'
  | 'SystemDesign'
  | 'CodingChallenge'
  | 'FinalRound';

export interface InterviewDto {
  id: string;
  applicationId: string;
  type: InterviewType;
  scheduledAt?: string | null;
  conductedAt?: string | null;
  notes?: string | null;
  feedback?: string | null;
}

export interface CreateInterviewDto {
  type: InterviewType;
  scheduledAt?: string | null;
  conductedAt?: string | null;
  notes?: string | null;
  feedback?: string | null;
}

export interface UpdateInterviewDto {
  type: InterviewType;
  scheduledAt?: string | null;
  conductedAt?: string | null;
  notes?: string | null;
  feedback?: string | null;
}
