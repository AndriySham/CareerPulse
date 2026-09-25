/**
 * Project DTOs matching backend production contracts.
 */

export interface ProjectDto {
  id: string;
  resumeRevisionId: string;
  projectName: string;
  role?: string | null;
  description?: string | null;
  techStack?: string | null;
  repositoryUrl?: string | null;
  liveDemoUrl?: string | null;
  createdAt?: string;
}

export interface CreateProjectDto {
  resumeRevisionId: string;
  projectName: string;
  role?: string | null;
  description?: string | null;
  techStack?: string | null;
  repositoryUrl?: string | null;
  liveDemoUrl?: string | null;
}

export interface UpdateProjectDto {
  projectName: string;
  role?: string | null;
  description?: string | null;
  techStack?: string | null;
  repositoryUrl?: string | null;
  liveDemoUrl?: string | null;
}
