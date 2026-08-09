import type { SkillCategory } from './masterSkill';

export type RevisionStatus = 'Draft' | 'Applied';

export interface PersonalInfo {
  fullName: string;
  email: string;
  phone?: string | null;
  linkedIn?: string | null;
  gitHub?: string | null;
  location?: string | null;
}

export interface ResumeRevisionSkillDto {
  masterSkillId: string;
  skillName: string;
  category: SkillCategory;
  proficiencyLevel: number;
}

export interface ResumeRevisionDto {
  id: string;
  status: RevisionStatus;
  personalInfo: PersonalInfo;
  professionalSummary: string;
  fileReference?: string | null;
  version: number;
  parentRevisionId?: string | null;
  createdAt: string;
  updatedAt: string;
  skills: ResumeRevisionSkillDto[];
}

export interface ResumeSkillInputDto {
  masterSkillId: string;
  proficiencyLevel: number;
}

export interface CreateResumeDraftDto {
  personalInfo: PersonalInfo;
  professionalSummary: string;
  skills: ResumeSkillInputDto[];
}

export interface UpdateResumeDraftDto {
  personalInfo: PersonalInfo;
  professionalSummary: string;
  skills: ResumeSkillInputDto[];
}
