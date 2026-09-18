/**
 * Education DTOs matching backend production contracts.
 * Note: Backend refactored Degree & FieldOfStudy into Description (max 200 chars).
 */

export interface EducationDto {
  id: string;
  resumeRevisionId: string;
  institutionName: string;
  description?: string | null;
  startYear?: number | null;
  endYear?: number | null;
}

export interface CreateEducationDto {
  resumeRevisionId: string;
  institutionName: string;
  description?: string | null;
  startYear?: number | null;
  endYear?: number | null;
}

export interface UpdateEducationDto {
  resumeRevisionId: string;
  institutionName: string;
  description?: string | null;
  startYear?: number | null;
  endYear?: number | null;
}
