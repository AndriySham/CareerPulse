export type SkillCategory =
  | 'ProgrammingLanguage'
  | 'Framework'
  | 'ORM'
  | 'Database'
  | 'Cloud'
  | 'DevOps'
  | 'Messaging'
  | 'Testing'
  | 'Tools'
  | 'SoftSkill'
  | 'Other';

export interface MasterSkillDto {
  id: string;
  name: string;
  category: SkillCategory;
  isActive: boolean;
  createdAt: string;
  aliases: string[];
}

export interface CreateMasterSkillDto {
  name: string;
  category: SkillCategory;
  aliases?: string[];
}
