import React from 'react';
import type { ResumeRevisionDto } from '@/types';
import ResumeStatusBadge from './ResumeStatusBadge';
import {
  FileText,
  Edit3,
  GitBranch,
  Mail,
  MapPin,
  Phone,
  Linkedin,
  Github,
  Award,
} from 'lucide-react';

interface ResumeCardProps {
  revision: ResumeRevisionDto;
  onView: (revision: ResumeRevisionDto) => void;
  onEdit: (revision: ResumeRevisionDto) => void;
  onSpawn: (revision: ResumeRevisionDto) => void;
  isSpawning?: boolean;
}

export const ResumeCard: React.FC<ResumeCardProps> = ({
  revision,
  onView,
  onEdit,
  onSpawn,
  isSpawning = false,
}) => {
  const isDraft = revision.status === 'Draft';
  const { personalInfo } = revision;

  return (
    <div className="group rounded-xl border border-border/60 bg-card p-5 shadow-sm transition-all hover:border-primary/40 hover:shadow-md flex flex-col justify-between">
      <div>
        {/* Header: Version badge + Status Badge */}
        <div className="flex items-center justify-between gap-2 mb-3">
          <div className="flex items-center gap-2">
            <span className="inline-flex items-center px-2.5 py-0.5 rounded-md bg-primary/10 text-primary text-xs font-bold border border-primary/20">
              v{revision.version}
            </span>
            {revision.parentRevisionId && (
              <span
                className="inline-flex items-center gap-1 text-[11px] text-muted-foreground bg-accent/50 px-2 py-0.5 rounded"
                title={`Spawned from revision ${revision.parentRevisionId}`}
              >
                <GitBranch className="h-3 w-3 text-muted-foreground" />
                Branched
              </span>
            )}
          </div>
          <ResumeStatusBadge status={revision.status} size="sm" />
        </div>

        {/* Developer Name & Title */}
        <h3 className="text-lg font-bold tracking-tight text-foreground group-hover:text-primary transition-colors">
          {personalInfo?.fullName || 'Untitled Profile'}
        </h3>

        {/* Contact Metadata Bar */}
        <div className="mt-2 flex flex-wrap items-center gap-y-1 gap-x-3 text-xs text-muted-foreground">
          {personalInfo?.email && (
            <span className="flex items-center gap-1">
              <Mail className="h-3 w-3 text-primary/70 shrink-0" />
              {personalInfo.email}
            </span>
          )}
          {personalInfo?.location && (
            <span className="flex items-center gap-1">
              <MapPin className="h-3 w-3 text-primary/70 shrink-0" />
              {personalInfo.location}
            </span>
          )}
          {personalInfo?.phone && (
            <span className="flex items-center gap-1">
              <Phone className="h-3 w-3 text-primary/70 shrink-0" />
              {personalInfo.phone}
            </span>
          )}
          {personalInfo?.linkedIn && (
            <span className="flex items-center gap-1">
              <Linkedin className="h-3 w-3 text-blue-400 shrink-0" />
              LinkedIn
            </span>
          )}
          {personalInfo?.gitHub && (
            <span className="flex items-center gap-1">
              <Github className="h-3 w-3 text-foreground shrink-0" />
              GitHub
            </span>
          )}
        </div>

        {/* Professional Summary snippet */}
        <p className="mt-3 text-xs text-muted-foreground/90 line-clamp-2 leading-relaxed bg-accent/20 p-2.5 rounded-lg border border-border/30 italic">
          "{revision.professionalSummary}"
        </p>

        {/* Skills Tag List */}
        {revision.skills && revision.skills.length > 0 && (
          <div className="mt-3">
            <div className="flex items-center gap-1 text-[11px] font-medium text-muted-foreground mb-1.5">
              <Award className="h-3 w-3 text-primary shrink-0" />
              <span>Skills ({revision.skills.length}):</span>
            </div>
            <div className="flex flex-wrap gap-1.5">
              {revision.skills.slice(0, 6).map((skill) => (
                <span
                  key={skill.masterSkillId}
                  className="inline-flex items-center gap-1 px-2 py-0.5 rounded-md bg-accent text-[11px] font-medium text-foreground border border-border/40"
                >
                  {skill.skillName}
                  <span className="text-[10px] text-primary font-bold">L{skill.proficiencyLevel}</span>
                </span>
              ))}
              {revision.skills.length > 6 && (
                <span className="inline-flex items-center px-1.5 py-0.5 rounded-md bg-accent/60 text-[11px] font-medium text-muted-foreground">
                  +{revision.skills.length - 6} more
                </span>
              )}
            </div>
          </div>
        )}
      </div>

      {/* Footer & Action Bar */}
      <div className="mt-4 pt-3 border-t border-border/40 flex items-center justify-between text-xs">
        <span className="text-[11px] text-muted-foreground">
          Updated {new Date(revision.updatedAt).toLocaleDateString()}
        </span>

        <div className="flex items-center gap-1.5">
          <button
            type="button"
            onClick={() => onView(revision)}
            className="inline-flex items-center gap-1 rounded-lg border border-border bg-background px-2.5 py-1.5 text-xs font-semibold text-foreground hover:bg-accent transition-colors cursor-pointer"
            title="View full resume snapshot details"
          >
            <FileText className="h-3.5 w-3.5 text-muted-foreground" />
            Details
          </button>

          {isDraft ? (
            <button
              type="button"
              onClick={() => onEdit(revision)}
              className="inline-flex items-center gap-1 rounded-lg bg-amber-500/10 border border-amber-500/30 px-2.5 py-1.5 text-xs font-semibold text-amber-400 hover:bg-amber-500/20 transition-colors cursor-pointer"
              title="Edit draft content and skills"
            >
              <Edit3 className="h-3.5 w-3.5" />
              Edit
            </button>
          ) : (
            <span
              className="inline-flex items-center gap-1 rounded-lg bg-accent/40 px-2.5 py-1.5 text-xs font-medium text-muted-foreground cursor-not-allowed opacity-60"
              title="Immutable revision linked to an Application. Use 'Spawn' to create a new editable draft."
            >
              <Edit3 className="h-3.5 w-3.5 opacity-40" />
              Locked
            </span>
          )}

          <button
            type="button"
            disabled={isSpawning}
            onClick={() => onSpawn(revision)}
            className="inline-flex items-center gap-1 rounded-lg bg-primary/10 border border-primary/20 px-2.5 py-1.5 text-xs font-semibold text-primary hover:bg-primary/20 transition-colors cursor-pointer disabled:opacity-50"
            title="Spawn a new editable draft version (Copy-on-Write per ADR 005)"
          >
            <GitBranch className="h-3.5 w-3.5" />
            Spawn
          </button>
        </div>
      </div>
    </div>
  );
};

export default ResumeCard;
