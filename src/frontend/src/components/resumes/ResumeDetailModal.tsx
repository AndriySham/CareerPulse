import React from 'react';
import Modal from '@/components/ui/Modal';
import ResumeStatusBadge from './ResumeStatusBadge';
import type { ResumeRevisionDto } from '@/types';
import {
  User,
  Mail,
  Phone,
  MapPin,
  Linkedin,
  Github,
  FileText,
  Award,
  GitBranch,
  Edit3,
  Lock,
  Calendar,
  CheckCircle,
} from 'lucide-react';

interface ResumeDetailModalProps {
  isOpen: boolean;
  onClose: () => void;
  revision: ResumeRevisionDto | null;
  onEdit: (revision: ResumeRevisionDto) => void;
  onSpawn: (revision: ResumeRevisionDto) => void;
  isSpawning?: boolean;
}

export const ResumeDetailModal: React.FC<ResumeDetailModalProps> = ({
  isOpen,
  onClose,
  revision,
  onEdit,
  onSpawn,
  isSpawning = false,
}) => {
  if (!revision) return null;

  const isDraft = revision.status === 'Draft';
  const { personalInfo } = revision;

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={`Resume Revision Details (Version ${revision.version})`}
      description={`Revision ID: ${revision.id}`}
      maxWidth="xl"
    >
      <div className="space-y-5">
        {/* Immutability & Status Alert Banner per ADR 005 */}
        {!isDraft ? (
          <div className="rounded-lg border border-emerald-500/30 bg-emerald-500/10 p-3.5 text-xs text-emerald-300 flex items-start gap-3">
            <Lock className="h-5 w-5 text-emerald-400 shrink-0 mt-0.5" />
            <div>
              <h4 className="font-semibold text-emerald-400 text-sm">
                Immutable Applied Revision (ADR 005)
              </h4>
              <p className="mt-0.5 opacity-90">
                This revision has been submitted with an application and is strictly read-only to guarantee historical application accuracy. To make edits, click <strong>Spawn New Version</strong> below to create a new draft copy (Copy-on-Write).
              </p>
            </div>
          </div>
        ) : (
          <div className="rounded-lg border border-amber-500/30 bg-amber-500/10 p-3.5 text-xs text-amber-300 flex items-start gap-3">
            <CheckCircle className="h-5 w-5 text-amber-400 shrink-0 mt-0.5" />
            <div>
              <h4 className="font-semibold text-amber-400 text-sm">Editable Draft Revision</h4>
              <p className="mt-0.5 opacity-90">
                This revision is in Draft status. You can modify its contact info, summary, or skills directly, or spawn a new version.
              </p>
            </div>
          </div>
        )}

        {/* Section 1: Overview Header */}
        <div className="flex flex-wrap items-center justify-between gap-3 rounded-lg border border-border/40 bg-accent/20 p-4">
          <div>
            <div className="flex items-center gap-2">
              <span className="text-xl font-bold text-foreground">
                {personalInfo?.fullName || 'Untitled Profile'}
              </span>
              <span className="inline-flex items-center px-2.5 py-0.5 rounded-md bg-primary/10 text-primary text-xs font-bold border border-primary/20">
                v{revision.version}
              </span>
            </div>
            <p className="text-xs text-muted-foreground mt-0.5">{personalInfo?.email}</p>
          </div>

          <div className="flex items-center gap-2">
            <ResumeStatusBadge status={revision.status} size="md" />
          </div>
        </div>

        {/* Section 2: Contact Information */}
        <div className="space-y-2 rounded-lg border border-border/40 bg-card p-4">
          <div className="flex items-center gap-2 text-xs font-semibold text-foreground border-b border-border/40 pb-2">
            <User className="h-4 w-4 text-primary" />
            <span>Contact Information</span>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-2.5 text-xs pt-1">
            {personalInfo?.email && (
              <div className="flex items-center gap-2 text-muted-foreground">
                <Mail className="h-3.5 w-3.5 text-primary shrink-0" />
                <span className="font-medium text-foreground">Email:</span> {personalInfo.email}
              </div>
            )}
            {personalInfo?.phone && (
              <div className="flex items-center gap-2 text-muted-foreground">
                <Phone className="h-3.5 w-3.5 text-primary shrink-0" />
                <span className="font-medium text-foreground">Phone:</span> {personalInfo.phone}
              </div>
            )}
            {personalInfo?.location && (
              <div className="flex items-center gap-2 text-muted-foreground">
                <MapPin className="h-3.5 w-3.5 text-primary shrink-0" />
                <span className="font-medium text-foreground">Location:</span> {personalInfo.location}
              </div>
            )}
            {personalInfo?.linkedIn && (
              <div className="flex items-center gap-2 text-muted-foreground truncate">
                <Linkedin className="h-3.5 w-3.5 text-blue-400 shrink-0" />
                <span className="font-medium text-foreground">LinkedIn:</span>
                <a
                  href={personalInfo.linkedIn}
                  target="_blank"
                  rel="noreferrer"
                  className="text-primary hover:underline truncate"
                >
                  {personalInfo.linkedIn}
                </a>
              </div>
            )}
            {personalInfo?.gitHub && (
              <div className="flex items-center gap-2 text-muted-foreground truncate">
                <Github className="h-3.5 w-3.5 text-foreground shrink-0" />
                <span className="font-medium text-foreground">GitHub:</span>
                <a
                  href={personalInfo.gitHub}
                  target="_blank"
                  rel="noreferrer"
                  className="text-primary hover:underline truncate"
                >
                  {personalInfo.gitHub}
                </a>
              </div>
            )}
          </div>
        </div>

        {/* Section 3: Professional Summary */}
        <div className="space-y-2 rounded-lg border border-border/40 bg-card p-4">
          <div className="flex items-center gap-2 text-xs font-semibold text-foreground border-b border-border/40 pb-2">
            <FileText className="h-4 w-4 text-primary" />
            <span>Professional Summary</span>
          </div>
          <p className="text-xs text-muted-foreground leading-relaxed whitespace-pre-line pt-1">
            {revision.professionalSummary}
          </p>
        </div>

        {/* Section 4: Attached Master Skills */}
        <div className="space-y-2 rounded-lg border border-border/40 bg-card p-4">
          <div className="flex items-center justify-between border-b border-border/40 pb-2">
            <div className="flex items-center gap-2 text-xs font-semibold text-foreground">
              <Award className="h-4 w-4 text-primary" />
              <span>Normalized Technical Skills (ADR 006)</span>
            </div>
            <span className="text-xs text-muted-foreground">
              {revision.skills?.length || 0} skills
            </span>
          </div>

          {revision.skills && revision.skills.length > 0 ? (
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-2 pt-1">
              {revision.skills.map((skill) => (
                <div
                  key={skill.masterSkillId}
                  className="flex items-center justify-between rounded-lg bg-accent/30 border border-border/40 px-3 py-2 text-xs"
                >
                  <div className="flex items-center gap-2">
                    <span className="font-semibold text-foreground">{skill.skillName}</span>
                    <span className="text-[10px] text-muted-foreground bg-accent px-1.5 py-0.5 rounded">
                      {skill.category}
                    </span>
                  </div>

                  <div className="flex items-center gap-1">
                    <span className="text-[11px] font-bold text-primary">
                      Level {skill.proficiencyLevel}/5
                    </span>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <p className="text-xs text-muted-foreground italic text-center py-2">
              No skills attached to this revision.
            </p>
          )}
        </div>

        {/* Section 5: Revision Metadata & Lineage */}
        <div className="rounded-lg border border-border/40 bg-accent/10 p-3 text-[11px] text-muted-foreground space-y-1">
          <div className="flex flex-wrap items-center justify-between gap-2">
            <div className="flex items-center gap-1.5">
              <Calendar className="h-3.5 w-3.5 text-primary shrink-0" />
              <span>Created: {new Date(revision.createdAt).toLocaleString()}</span>
            </div>
            <div className="flex items-center gap-1.5">
              <span>Updated: {new Date(revision.updatedAt).toLocaleString()}</span>
            </div>
          </div>
          {revision.parentRevisionId && (
            <div className="flex items-center gap-1.5 pt-1 border-t border-border/20 text-primary">
              <GitBranch className="h-3.5 w-3.5 shrink-0" />
              <span>Spawned from parent revision ID: {revision.parentRevisionId}</span>
            </div>
          )}
        </div>

        {/* Actions Footer */}
        <div className="flex items-center justify-between pt-3 border-t border-border/40">
          <button
            type="button"
            onClick={onClose}
            className="rounded-lg border border-border bg-background px-4 py-2 text-sm font-semibold text-foreground hover:bg-accent transition-colors"
          >
            Close
          </button>

          <div className="flex items-center gap-2">
            {isDraft && (
              <button
                type="button"
                onClick={() => {
                  onClose();
                  onEdit(revision);
                }}
                className="inline-flex items-center gap-1.5 rounded-lg bg-amber-500/10 border border-amber-500/30 px-4 py-2 text-sm font-semibold text-amber-400 hover:bg-amber-500/20 transition-colors cursor-pointer"
              >
                <Edit3 className="h-4 w-4" />
                Edit Draft
              </button>
            )}

            <button
              type="button"
              disabled={isSpawning}
              onClick={() => {
                onSpawn(revision);
              }}
              className="inline-flex items-center gap-1.5 rounded-lg bg-primary px-4 py-2 text-sm font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all cursor-pointer disabled:opacity-50"
            >
              <GitBranch className="h-4 w-4" />
              Spawn New Version
            </button>
          </div>
        </div>
      </div>
    </Modal>
  );
};

export default ResumeDetailModal;
