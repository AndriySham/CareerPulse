import React from 'react';
import type { ProjectDto } from '@/types';
import { FolderGit2, Edit3, Trash2, Github, Globe, ExternalLink, Code2 } from 'lucide-react';

interface ProjectCardProps {
  project: ProjectDto;
  onEdit: (project: ProjectDto) => void;
  onDelete: (project: ProjectDto) => void;
  isReadOnly?: boolean;
}

export const ProjectCard: React.FC<ProjectCardProps> = ({
  project,
  onEdit,
  onDelete,
  isReadOnly = false,
}) => {
  const techStackList = project.techStack
    ? project.techStack
        .split(',')
        .map((s) => s.trim())
        .filter(Boolean)
    : [];

  return (
    <div className="group relative rounded-xl border border-border/80 bg-background p-4 shadow-xs hover:border-border transition-all flex flex-col justify-between gap-3">
      <div className="flex items-start justify-between gap-3">
        <div className="flex items-start gap-3 flex-1 min-w-0">
          <div className="p-2 rounded-lg bg-primary/10 text-primary border border-primary/20 shrink-0 mt-0.5">
            <FolderGit2 className="h-4 w-4" />
          </div>
          <div className="space-y-1.5 flex-1 min-w-0">
            <div className="flex flex-wrap items-center gap-2">
              <h3 className="font-semibold text-sm text-foreground leading-snug">
                {project.projectName}
              </h3>
              {project.role && (
                <span className="inline-flex items-center px-2 py-0.5 rounded-md text-[11px] font-medium bg-secondary text-secondary-foreground border border-border/50">
                  {project.role}
                </span>
              )}
            </div>

            {project.description && (
              <p className="text-xs text-muted-foreground whitespace-pre-line leading-relaxed">
                {project.description}
              </p>
            )}

            {/* Tech Stack Tags */}
            {techStackList.length > 0 && (
              <div className="flex flex-wrap items-center gap-1.5 pt-1">
                {techStackList.map((tech, idx) => (
                  <span
                    key={idx}
                    className="inline-flex items-center gap-1 px-2 py-0.5 rounded-md text-[10px] font-medium bg-accent/60 text-accent-foreground border border-border/40"
                  >
                    <Code2 className="h-2.5 w-2.5 text-primary" />
                    {tech}
                  </span>
                ))}
              </div>
            )}

            {/* External Links */}
            {(project.repositoryUrl || project.liveDemoUrl) && (
              <div className="flex flex-wrap items-center gap-4 pt-1 text-xs">
                {project.repositoryUrl && (
                  <a
                    href={project.repositoryUrl}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="inline-flex items-center gap-1.5 text-muted-foreground hover:text-primary transition-colors font-medium"
                  >
                    <Github className="h-3.5 w-3.5" />
                    <span>Repository</span>
                    <ExternalLink className="h-2.5 w-2.5 opacity-60" />
                  </a>
                )}

                {project.liveDemoUrl && (
                  <a
                    href={project.liveDemoUrl}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="inline-flex items-center gap-1.5 text-muted-foreground hover:text-primary transition-colors font-medium"
                  >
                    <Globe className="h-3.5 w-3.5" />
                    <span>Live Demo</span>
                    <ExternalLink className="h-2.5 w-2.5 opacity-60" />
                  </a>
                )}
              </div>
            )}
          </div>
        </div>

        {/* Action buttons (only when not read-only) */}
        {!isReadOnly && (
          <div className="flex items-center gap-1 shrink-0">
            <button
              type="button"
              onClick={() => onEdit(project)}
              className="p-1.5 rounded-lg text-muted-foreground hover:text-foreground hover:bg-accent transition-colors cursor-pointer"
              title="Edit Project"
            >
              <Edit3 className="h-3.5 w-3.5" />
            </button>
            <button
              type="button"
              onClick={() => onDelete(project)}
              className="p-1.5 rounded-lg text-muted-foreground/60 hover:text-destructive hover:bg-destructive/10 transition-colors cursor-pointer"
              title="Delete Project"
            >
              <Trash2 className="h-3.5 w-3.5" />
            </button>
          </div>
        )}
      </div>
    </div>
  );
};

export default ProjectCard;
