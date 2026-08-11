import type { ResumeTrack, CareerLevel, PersonalInfo, ResumeSkillInputDto } from './resume';
import type { SkillCategory } from './masterSkill';

export interface ScoredField<T> {
  value?: T | null;
  confidenceScore: number; // 0.0 - 100.0
  needsReview: boolean;
  extractionNote?: string | null;
}

export type SkillResolutionStatus = 'AutoResolved' | 'NeedsUserInput' | 'Ignored';

export interface ScoredSkillDto {
  rawText: string;
  resolvedMasterSkillId?: string | null;
  resolvedMasterSkillName?: string | null;
  confidenceScore: number;
  resolutionStatus: SkillResolutionStatus;
}

export interface WorkExperienceDto {
  companyName: string;
  positionTitle: string;
  startMonth: number;
  startYear: number;
  endMonth?: number | null;
  endYear?: number | null;
  isCurrentJob: boolean;
  description?: string | null;
  achievements?: string | null;
  techStack?: string | null;
}

export interface ResumeImportResultDto {
  professionalSummary: ScoredField<string>;
  personalInfo: ScoredField<PersonalInfo>;
  extractedSkills: ScoredSkillDto[];
  workExperiences: ScoredField<WorkExperienceDto>[];
  rawWarnings: string[];
  uploadedFileReference?: string | null;
}

export type SkillResolutionAction = 'CreateNew' | 'MapToExisting' | 'Ignore';

export interface SkillResolutionDecisionDto {
  rawText: string;
  action: SkillResolutionAction;
  targetMasterSkillId?: string | null;
  newSkillName?: string | null;
  newSkillCategory?: SkillCategory | null;
}

export interface ConfirmedResumeImportDto {
  name: string;
  track: ResumeTrack;
  careerLevel: CareerLevel;
  targetRole: string;
  professionalSummary: string;
  personalInfo: PersonalInfo;
  skills: ResumeSkillInputDto[];
  uploadedFileReference?: string | null;
}
