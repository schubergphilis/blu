
require 'spec_helper'

describe 'blu::service' do
  it 'has a running service of Blu Service' do
    expect(service('BluService')).to be_running
  end

  describe file('c:\Blu\BluService.exe') do
    it { should be_file }
  end
end
