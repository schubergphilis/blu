require "chef/resource/windows_script"

class Chef
  class Resource
    class BluScript < Chef::Resource::WindowsScript
      provides :blu_script, os: 'windows'

      allowed_actions [:create_runspace, :dispose_runspace]

      def initialize(name, run_context = nil)
        super(name, run_context, nil, 'blushell.exe')
        @credentials = nil
      end

      def runspace(arg = nil)
        set_or_return(
            :runspace,
            arg,
            :kind_of => [ String ]
        )
      end

      def credentials(arg = nil)
        set_or_return(
            :credentials,
            arg,
            :kind_of => [ String ]
        )
      end
    end
  end
end
