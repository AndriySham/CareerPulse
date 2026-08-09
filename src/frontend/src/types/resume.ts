export type RevisionStatus = 'Draft' | 'Applied';

export interface ResumeRevisionDto {
  id: string;
  status: RevisionStatus;
  professionalSummary: string;
  version: number;
  parentRevisionId?: string | null;
  fileReference?: string | null;
  createdAt: string;
  updatedAt: string;
}
